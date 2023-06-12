using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PICOFacialTrackerVamLink;

public partial class App : Application
{
    private static Pico4SAFTExtTrackingModule? module;
    private static Logger? logger;

    [STAThread]
    static void Main()
    {
        // TODO Whatever you want to do before starting
        // the WPF application and loading all WPF dlls
        RunApp();
    }

    // Ensure the method is not inlined, so you don't
    // need to load any WPF dll in the Main method
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static void RunApp()
    {
        var app = new App();
        //app.InitializeComponent();

        MainWindow mainWindow = new MainWindow();
        logger = new Logger(mainWindow, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.log"));
        new Thread(new ThreadStart(InitializeModule)).Start();

        app.Run(mainWindow);
    }

    static void InitializeModule()
    {
        module = new Pico4SAFTExtTrackingModule(logger);
        module.Initialize();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        module?.Dispose();
        logger?.Dispose();

        base.OnExit(e);
    }
}
