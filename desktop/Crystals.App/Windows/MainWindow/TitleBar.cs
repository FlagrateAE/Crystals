using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core.Utilities;
using Wpf.Ui.Controls;

namespace Crystals.App.Windows.MainWindow;

public partial class MainWindow
{
    private Border TitleBar()
    {
        var titleBar = new TitleBar
        {
            Title = "Flagrate Crystals",
            Icon = new ImageIcon
            {
                Source = IconLoader.LoadDefaultIcon(128)
            },
            ShowMaximize = false,
            ShowMinimize = false,
        };
        
        var borderWrapper = new Border
        {
            BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 }),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Child = titleBar
        };

        return borderWrapper;
    }
}