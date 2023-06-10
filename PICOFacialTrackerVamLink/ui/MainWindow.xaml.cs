using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        this.Dispatcher.Invoke(new Action(() => {
            Btn.Visibility = Visibility.Hidden;
        }));
    }

    public void ShowButton(string btnMsg, LogDisplayer.ButtonPressed callback)
    {
        this.Dispatcher.Invoke(new Action(() => {
            Btn.Content = btnMsg;
            this.callback = callback;
        }));
    }

    public void ShowText(string text, LogDisplayer.Color color)
    {
        this.Dispatcher.Invoke(new Action(() => {
            Text.Text = text;
            Text.Foreground = color.Equals(LogDisplayer.Color.BLACK) ? Brushes.Black : Brushes.Red;
        }));
    }

    private void Btn_Click(object sender, RoutedEventArgs e)
    {
        this.Dispatcher.Invoke(new Action(() => {
            this.callback?.OnButtonPressed();
        }));
    }
}