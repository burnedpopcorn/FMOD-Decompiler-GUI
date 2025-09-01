using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FMOD_DecompilerUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ExitMenu_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
    private void GUI_Redirect_Click(object sender, RoutedEventArgs e)
    {
        OpenBrowser("https://github.com/burnedpopcorn/FMOD-Decompiler-GUI");
    }
    private void CMD_Redirect_Click(object sender, RoutedEventArgs e)
    {
        OpenBrowser("https://github.com/doggywatty/FMOD-Decompiler");
    }

    /// From https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Dialogs/AboutAvaloniaDialog.xaml.cs
    public static void OpenBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var process = Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = "/bin/sh",
                        Arguments = $"-c \"{$"xdg-open {url}".Replace("\"", "\\\"")}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                )) { }
            }
            else
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"{url}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                })) { }
            }
        }
        catch { }
    }
}
