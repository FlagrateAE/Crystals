using System.Windows;
using System.Windows.Controls;
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
    private readonly CrystalsEngine _engine;
    private readonly MainWindow _mainWindow;

    private NotifyIcon? _trayIcon;
    private BitmapSource _iconImage;

    private bool _isExiting;

    public TrayIconService(CrystalsEngine engine, MainWindow mainWindow)
    {
        _engine = engine;
        _mainWindow = mainWindow;

        _engine.OnStateChanged += OnEngineStateChanged;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(InitializeIcon);
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
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
        SetIcon(ImageLoader.LoadFromUri(ImageLoader.IconUris.AE, 64));

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


    private void OnEngineStateChanged(ISource? source, CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var newIcon = DrawingHelper.RecolorIcon(source != null ? source.Icon : _iconImage, color);
            SetIcon(newIcon);
        });
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