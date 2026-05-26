using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Crystals.App.Windows;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Crystals.Core.Utilities;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Crystals.App.Services;

public class TrayIconService : BackgroundService
{
    private readonly Uri _defaultIconUri = new("pack://application:,,,/Resources/Icon.png");

    private readonly CrystalsEngine _engine;
    private readonly MainWindow _mainWindow;

    private NotifyIcon? _trayIcon;
    private BitmapSource? _iconImage;
    private IntPtr _hwnd;
    private const int HotkeyId = 9000;
    private bool _isExiting;

    public TrayIconService(CrystalsEngine engine, MainWindow mainWindow)
    {
        _engine = engine;
        _mainWindow = mainWindow;

        _engine.OnSourceFocused += OnEngineSourceFocused;
        _engine.OnEngineColorChanged += OnEngineColorChanged;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(InitializeIcon);
        Application.Current.Dispatcher.Invoke(RegisterHotkey);
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ComponentDispatcher.ThreadFilterMessage -= OnMessage;
            Win32Hotkeys.UnregisterHotKey(_hwnd, HotkeyId);

            if (_trayIcon == null) return;
            _trayIcon.Unregister();
            _trayIcon.Dispose();
        });

        await Task.CompletedTask;
    }

    private void InitializeIcon()
    {
        _mainWindow.ShowInTaskbar = false;
        _mainWindow.Show();
        _mainWindow.Hide();
        _mainWindow.ShowInTaskbar = true;

        _trayIcon = new NotifyIcon
        {
            TooltipText = "Flagrate Crystals",
            Visibility = Visibility.Visible
        };
        SetIcon(IconLoader.LoadFromUri(_defaultIconUri));

        var contextMenu = new ContextMenu();

        var exitItem = new MenuItem
        {
            Header = "Exit",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Dismiss24 }
        };
        exitItem.Click += (_, _) =>
        {
            _isExiting = true;
            Application.Current.Shutdown();
        };

        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(exitItem);

        _trayIcon.Menu = contextMenu;

        if (_mainWindow.Content is Panel rootPanel)
        {
            rootPanel.Children.Add(_trayIcon);
        }

        _mainWindow.Closing += (_, e) =>
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                _mainWindow.Hide();
            }
        };

        _trayIcon.LeftClick += (_, _) => ShowMainWindow();
        _trayIcon.Register();
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

    private void OnEngineSourceFocused(ISource source)
    {
        Application.Current.Dispatcher.Invoke(() => { SetIcon(source.Icon); });
    }

    private void OnEngineColorChanged(CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { SetIcon(DrawingHelper.RecolorIcon(_iconImage!, color)); });
    }

    private void OnMessage(ref MSG msg, ref bool handled)
    {
        if (msg.message == Win32Hotkeys.WM_HOTKEY && msg.wParam.ToInt32() == HotkeyId)
        {
            ShowMainWindow();
            handled = true;
        }
    }

    private void SetIcon(BitmapSource icon)
    {
        _iconImage = icon;
        _trayIcon?.Icon = _iconImage;
    }

    private void ShowMainWindow()
    {
        _mainWindow.Show();
        if (_mainWindow.WindowState == WindowState.Minimized)
        {
            _mainWindow.WindowState = WindowState.Normal;
        }

        _mainWindow.Activate();
    }
}