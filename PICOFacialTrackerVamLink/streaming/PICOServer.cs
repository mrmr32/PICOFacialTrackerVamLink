using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PICOFacialTrackerVamLink;

public sealed class PICOServer : IDisposable, DataProvider<BlendShape>
{
    private static readonly unsafe int pxrHeaderSize = sizeof(TrackingDataHeader);
    private readonly int PacketIndex = pxrHeaderSize;
    private static readonly unsafe int pxrFtInfoSize = sizeof(PxrFTInfo);
    private static readonly int PacketSize = pxrHeaderSize + pxrFtInfoSize;

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private PxrFTInfo data;

    private LogDisplayer logger;

    private bool stopped;
    private Thread? thread;

    private IDictionary<BlendShape, float>? latestData, latestGotData;
    private object dataLock;

    public PICOServer(string IP_ADDRESS, int PORT_NUMBER, LogDisplayer logger)
    {
        this.udpClient = new UdpClient(PORT_NUMBER);
        this.endPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), PORT_NUMBER);

        this.logger = logger;

        this.dataLock = new object();
    }

    public void PerformHandshake()
    {
        unsafe
        {
            fixed (PxrFTInfo* pData = &data)
                ReceivePxrData(pData);
        }
    }

    private unsafe bool ReceivePxrData(PxrFTInfo* pData)
    {
        fixed (byte* ptr = udpClient!.Receive(ref endPoint))
        {
            TrackingDataHeader tdh;
            Buffer.MemoryCopy(ptr, &tdh, pxrHeaderSize, pxrHeaderSize);
            if (tdh.tracking_type != 2) return false; // not facetracking packet

            Buffer.MemoryCopy(ptr + PacketIndex, pData, pxrFtInfoSize, pxrFtInfoSize);
        }
        return true;
    }

    public void Update()
    {
        this.stopped = false;

        this.thread = new Thread(new ThreadStart(this._Update));
        this.thread.Start();
    }


    private void _Update()
    {
        try
        {
            while (!this.stopped)
            {
                unsafe
                {
                    fixed (PxrFTInfo* pData = &data)
                    {
                        if (ReceivePxrData(pData))
                        {
                            float* pxrShape = pData->blendShapeWeight;
                            var data = new Dictionary<BlendShape, float>();

                            var values = Enum.GetValues(typeof(BlendShape));
                            for (int n = 0; n < values.Length; n++)
                            {
                                data.Add((BlendShape)values.GetValue(n), pxrShape[n]);
                            }

                            this.SetData(data);
                        }
                    }
                }
            }
        }
        catch (SocketException ex) when (ex.ErrorCode is 10060)
        {
            /*if (!StreamerValidity()) this.logger.ShowText("Streaming Assistant is currently not running. Please ensure Streaming Assistant is running to send tracking data.", LogDisplayer.Color.RED);
            else*/ this.logger.ShowText("Data was not sent within the timeout. " + ex.Message.ToString(), LogDisplayer.Color.RED);
        }
        catch (Exception ex)
        {
            this.logger.ShowText("Unexpected exceptions: " + ex.ToString(), LogDisplayer.Color.RED);
        }

        this.udpClient.Client.ReceiveTimeout = 5000;
    }

    public void Dispose()
    {
        this.stopped = true;
        this.thread?.Join();

        udpClient.Client.Blocking = false;
        udpClient.Dispose();
    }

    public bool IsNewDataAvailable()
    {
        lock (this.dataLock)
        {
            if (this.latestData == null) return false;
            return (this.latestData != this.latestGotData);
        }
    }

    public IDictionary<BlendShape, float> GetData()
    {
        lock (this.dataLock)
        {
            this.latestGotData = this.latestData; // save for the `IsNewDataAvailable`
            return new Dictionary<BlendShape, float>(this.latestData);
        }
    }

    private void SetData(IDictionary<BlendShape, float> data)
    {
        lock (this.dataLock)
        {
            this.latestData = data;
        }
    }
}
