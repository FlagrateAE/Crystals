using System.Windows;
using System.Windows.Controls;
using Crystals.App.Controls;
using Crystals.Core;
using Crystals.Core.Utilities;
using Wpf.Ui.Controls;

namespace Crystals.App.Windows;

public partial class MainWindow : FluentWindow
{
    private const int WindowWidth = 1054;
    private const int WindowHeight = 600;

    public MainWindow(CrystalsEngine engine)
    {
        Title = "Flagrate Crystals";
        Icon = ImageLoader.LoadFromUri(ImageLoader.IconUris.AE, 128);

        Width = WindowWidth;
        MinWidth = WindowWidth;
        MaxWidth = WindowWidth;
        Height = WindowHeight;
        MinHeight = WindowHeight;
        MaxHeight = WindowHeight;

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ExtendsContentIntoTitleBar = true;
        ResizeMode = ResizeMode.NoResize;
        WindowBackdropType = WindowBackdropType.Mica;

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var titleBar = TitleBar();
        Grid.SetRow(titleBar, 0);

        var content = new Grid();
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(content, 1);

        var sourcesPanel = new SourcesPanel(engine);
        Grid.SetColumn(sourcesPanel, 0);
        content.Children.Add(sourcesPanel);

        var devicesPanel = new DevicesPanel(engine);
        Grid.SetColumn(devicesPanel, 1);
        content.Children.Add(devicesPanel);

        var engineConnector = new EngineConnector(sourcesPanel, engine, devicesPanel);
        Grid.SetColumnSpan(engineConnector, 2);
        content.Children.Add(engineConnector);

        root.Children.Add(titleBar);
        root.Children.Add(content);

        Content = root;
    }
}