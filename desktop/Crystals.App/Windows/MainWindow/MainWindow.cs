using System.Windows;
using System.Windows.Controls;
using Crystals.App.Controls;
using Crystals.Core;
using Crystals.Core.Utilities;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Crystals.App.Windows.MainWindow;

public partial class MainWindow : FluentWindow
{
    private const int WindowWidth = 1054;
    private const int WindowHeight = 600;

    public MainWindow(CrystalsEngine engine)
    {
        Title = "Flagrate Crystals";
        Icon = ImageLoader.LoadDefaultIcon(128);

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
        // root.ShowGridLines = true;
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var titleBar = TitleBar();
        Grid.SetRow(titleBar, 0);

        var content = new Grid();
        content.ShowGridLines = true;
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(content, 1);

        var devicesPanel = new DevicesPanel(engine);
        Grid.SetColumn(devicesPanel, 0);
        content.Children.Add(devicesPanel);
        
        var sourcesPanel = new SourcesPanel(engine);
        Grid.SetColumn(sourcesPanel, 1);
        content.Children.Add(sourcesPanel);

        root.Children.Add(titleBar);
        root.Children.Add(content);

        Content = root;
    }
}