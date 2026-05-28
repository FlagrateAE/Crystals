using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Crystals.App.Windows;
using Crystals.Core.Utilities;
using Microsoft.Extensions.Hosting;

namespace Crystals.App.Services;

public class HotkeyService(MainWindow mainWindow) : BackgroundService
{
    private const int HotkeyId = 9000;
    private readonly IntPtr _hwnd;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(RegisterHotkey);
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ComponentDispatcher.ThreadFilterMessage -= OnMessage;
            Win32Hotkeys.UnregisterHotKey(_hwnd, HotkeyId);
        });

        await Task.CompletedTask;
    }

    private void OnMessage(ref MSG msg, ref bool handled)
    {
        if (msg.message == Win32Hotkeys.WM_HOTKEY && msg.wParam.ToInt32() == HotkeyId)
        {
            ToggleMainWindowVisibility();
            handled = true;
        }
    }

    private void RegisterHotkey()
    {
        bool success = Win32Hotkeys.RegisterHotKey(
            _hwnd, HotkeyId,
            Win32Hotkeys.MOD_WIN | Win32Hotkeys.MOD_ALT | Win32Hotkeys.MOD_SHIFT, 0x7B
        );

        if (!success)
        {
            int error = Marshal.GetLastWin32Error();
            Console.WriteLine($"Failed to register hotkey. Error code: {error}");
        }

        ComponentDispatcher.ThreadFilterMessage += OnMessage;
    }

    private void ToggleMainWindowVisibility()
    {
        if (mainWindow.Visibility == Visibility.Visible)
        {
            mainWindow.Hide();
        }
        else
        {
            mainWindow.Show();
        }
    }
}