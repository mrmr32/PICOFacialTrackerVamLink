using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PICOFacialTrackerVamLink;

public class Logger : LogDisplayer, LogDisplayer.ButtonPressed, IDisposable
{
    private string filePath;
    private LogDisplayer intercepting;

    private string btnText;
    private LogDisplayer.ButtonPressed? btnCallback;

    private StreamWriter? streamWriter;

    public Logger(LogDisplayer intercept, string path)
    {
        this.intercepting = intercept;
        this.btnText = "";

        this.filePath = path;

        try
        {
            if (!File.Exists(path)) this.streamWriter = File.CreateText(path);
            else this.streamWriter = File.AppendText(this.filePath);
        }
        catch (IOException ex)
        {
            intercept.ShowText("Couldn't open log files, reason: " + ex.Message);
        }
    }

    public void Dispose()
    {
        this.streamWriter?.Dispose();
    }

    public void HideButton()
    {
        this.intercepting.HideButton();
    }

    public void OnButtonPressed()
    {
        this.streamWriter.WriteLine(DateTime.Now.ToString() + " - Button {} pressed", this.btnText);
        this.streamWriter.Flush();

        // call the real callback
        this.btnCallback?.OnButtonPressed();
    }

    public void ShowButton(string btnMsg, LogDisplayer.ButtonPressed callback)
    {
        this.btnText = btnMsg;
        this.btnCallback = callback;

        this.intercepting.ShowButton(btnMsg, this);
    }

    public void ShowText(string text, LogDisplayer.Color color = LogDisplayer.Color.BLACK)
    {
        this.streamWriter.WriteLine(DateTime.Now.ToString() + " - New message: " + text);
        this.streamWriter.Flush();

        this.intercepting.ShowText(text, color);
    }
}
