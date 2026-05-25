using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Crystals.App.Windows;
using Crystals.Core;
using Crystals.Core.Models;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Crystals.App.Services;

public class TrayIconService(CrystalsEngine engine, MainWindow mainWindow) : BackgroundService
{
    private NotifyIcon? _trayIcon;
    private bool _isExiting;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Tray icon service started");
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
        var imageUri = new Uri("pack://application:,,,/Assets/icon.png", UriKind.RelativeOrAbsolute);

        _trayIcon = new NotifyIcon
        {
            TooltipText = "Flagrate Crystals",
            Icon = BitmapFrame.Create(imageUri),
            Visibility = Visibility.Visible
        };

        var contextMenu = new ContextMenu();

        var overrideRedItem = new MenuItem
        {
            Header = "Override: Red",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Color24 }
        };
        overrideRedItem.Click += (_, _) => { engine.ManualOverride(new CrystalsColor(0f, 1f, 1f)); };

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

        if (mainWindow.Content is Panel rootPanel)
        {
            rootPanel.Children.Add(_trayIcon);
        }

        mainWindow.Closing += (_, e) =>
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                mainWindow.Hide();
            }
        };

        _trayIcon.LeftDoubleClick += (_, _) =>
        {
            mainWindow.Show();
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }

            mainWindow.Activate();
        };

        _trayIcon.Register();
    }
}