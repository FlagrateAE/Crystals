using System.Windows;
using System.Windows.Controls;
using Crystals.App.Controls;
using Crystals.Core.Utilities;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Crystals.App.Windows.MainWindow;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        Title = "Flagrate Crystals";
        Width = 400;
        Height = 450;
        MinWidth = 400;
        MaxWidth = 400;
        MinHeight = 450;
        MaxHeight = 450;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        WindowBackdropType = WindowBackdropType.Mica;
        ExtendsContentIntoTitleBar = true;
        ResizeMode = ResizeMode.NoResize;
        Icon = IconLoader.LoadDefaultIcon(128);

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
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var content = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        content.Children.Add(hueSelector);
        content.Children.Add(statusLabel);

        var titleBar = TitleBar();

        Grid.SetRow(titleBar, 0);
        Grid.SetRow(content, 1);

        root.Children.Add(titleBar);
        root.Children.Add(content);

        Content = root;
    }
}