using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PICOFacialTrackerVamLink;

public sealed class VAMClient<T> : IDisposable
{
    public sealed class State
    {
        public byte[] buffer = new byte[bufSize];
    }

    private VAMConverter<T> provider;
    private LogDisplayer logger;

    private Socket socket;
    private const int bufSize = 8 * 1024;
    private State state;

    private bool stopped;
    private Thread? thread;

    public VAMClient(string IP_ADDRESS, int PORT_NUMBER, VAMConverter<T> provider, LogDisplayer logger)
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this.state = new State();

        this.provider = provider;

        try
        {
            this.socket.Connect(IPAddress.Parse(IP_ADDRESS), PORT_NUMBER);
        }
        catch (Exception ex)
        {
            this.logger.ShowText("Unexpected exceptions: " + ex.ToString(), LogDisplayer.Color.RED);
        }
    }

    public void Update()
    {
        this.stopped = false;

        this.thread = new Thread(new ThreadStart(this._Update));
        this.thread.Start();
    }


    private void _Update()
    {
        while (!stopped)
        {
            try
            {
                if (!this.provider.IsNewDataAvailable()) continue; // nothing to send

                byte[] data = Encoding.ASCII.GetBytes(this.provider.GetJSONData());
                this.socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = this.socket.EndSend(ar);
                }, state);
            }
            catch (Exception ex)
            {
                this.logger.ShowText("Unexpected exception: " + ex.ToString(), LogDisplayer.Color.RED);
            }
        }
    }

    public void Dispose()
    {
        this.stopped = true;
        this.thread?.Join();

        this.socket.Dispose();
    }
}