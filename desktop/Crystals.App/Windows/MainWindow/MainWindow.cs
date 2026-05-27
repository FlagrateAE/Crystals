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
        Icon = IconLoader.LoadDefaultIcon(128);

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

        var hueSelector = new CustomColorPicker
        {
            Width = 250,
            Height = 250,
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var statusLabel = new TextBlock
        {
            Text = $"Selected Hue: {hueSelector.Color}",
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(10),
            Appearance = TextColor.Primary
        };

        hueSelector.HueChanged += (_, e) => { statusLabel.Text = $"Selected Hue: {e:F0}°"; };

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

        root.Children.Add(titleBar);
        root.Children.Add(content);

        Content = root;
    }
}