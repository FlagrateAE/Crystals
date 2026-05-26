using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Crystals.App.Windows;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Crystals.Core.Utilities;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Crystals.App.Services;

public class TrayIconService : BackgroundService
{
    private readonly Uri _defaultIconUri = new("pack://application:,,,/Resources/icon.png");

    private readonly CrystalsEngine _engine;
    private readonly MainWindow _mainWindow;

    private NotifyIcon? _trayIcon;
    private BitmapSource? _iconImage;
    private bool _isExiting;

    public TrayIconService(CrystalsEngine engine, MainWindow mainWindow)
    {
        _engine = engine;
        _mainWindow = mainWindow;

        _engine.OnSourceFocused += OnEngineSourceFocused;
        _engine.OnEngineColorChanged += OnEngineColorChanged;

        _iconImage = BitmapFrame.Create(_defaultIconUri);
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
        _trayIcon = new NotifyIcon
        {
            TooltipText = "Flagrate Crystals",
            Icon = BitmapFrame.Create(_defaultIconUri),
            Visibility = Visibility.Visible
        };

        var contextMenu = new ContextMenu();

        var overrideRedItem = new MenuItem
        {
            Header = "Override: Red",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Color24 }
        };
        overrideRedItem.Click += (_, _) => { _engine.ManualOverride(new CrystalsColor(0f, 1f, 1f)); };

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

        contextMenu.Items.Add(overrideRedItem);
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

        _trayIcon.LeftDoubleClick += (_, _) =>
        {
            _mainWindow.Show();
            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                _mainWindow.WindowState = WindowState.Normal;
            }

            _mainWindow.Activate();
        };

        _trayIcon.Register();
    }

    private void OnEngineSourceFocused(ISource source)
    {
        Application.Current.Dispatcher.Invoke(() => { SetIcon(source.Icon); });
    }

    private void OnEngineColorChanged(CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { SetIcon(RecolorIcon(_iconImage!, color)); });
    }

    private void SetIcon(BitmapSource icon)
    {
        _iconImage = icon;
        _trayIcon?.Icon = _iconImage;
    }

    private BitmapSource RecolorIcon(BitmapSource icon, CrystalsColor color)
    {
        return DrawingHelper.RecolorIcon(icon, color);
    }
}