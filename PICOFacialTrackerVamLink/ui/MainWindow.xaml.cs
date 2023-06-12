using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PICOFacialTrackerVamLink;

public partial class MainWindow : Window, LogDisplayer
{
    private LogDisplayer.ButtonPressed? callback;

    public MainWindow()
    {
        this.callback = null;

        InitializeComponent();
    }

    public void HideButton()
    {
        try
        {
            this.Dispatcher.Invoke(new Action(() => {
                Btn.Visibility = Visibility.Hidden;
            }));
        }
        catch (TaskCanceledException ignore) { }
        catch (ThreadInterruptedException ignore) { }
    }

    public void ShowButton(string btnMsg, LogDisplayer.ButtonPressed callback)
    {
        try
        {
            this.Dispatcher.Invoke(new Action(() => {
                Btn.Content = btnMsg;
                this.callback = callback;
            }));
        }
        catch (TaskCanceledException ignore) { }
        catch (ThreadInterruptedException ignore) { }
    }

    public void ShowText(string text, LogDisplayer.Color color)
    {
        try
        {
            this.Dispatcher.Invoke(new Action(() => {
                Text.Text = text;
                Text.Foreground = color.Equals(LogDisplayer.Color.BLACK) ? Brushes.Black : Brushes.Red;
            }));
        }
        catch (TaskCanceledException ignore) { }
        catch (ThreadInterruptedException ignore) { }
    }

    private void Btn_Click(object sender, RoutedEventArgs e)
    {
        this.callback?.OnButtonPressed();
    }
}