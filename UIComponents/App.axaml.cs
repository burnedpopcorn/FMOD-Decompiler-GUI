using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using FMOD_DecompilerUI.ViewModels;
using FMOD_DecompilerUI.Views;

using System.Diagnostics;
using System.Linq;

namespace FMOD_DecompilerUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += OnAppExit;//run OnAppExit below
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    private void OnAppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // Kill CMD FMOD-Decompiler if it's still running
        using (var childProcess = Process.GetProcessesByName("FMOD-Decompiler").FirstOrDefault())
        {
            if (childProcess != null && !childProcess.HasExited)
            {
                childProcess.Kill();
                childProcess.Dispose();
            }
        }
    }
}
