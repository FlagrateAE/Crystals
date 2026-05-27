using Crystals.Core.Utilities;
using Wpf.Ui.Controls;

namespace Crystals.App.Windows.MainWindow;

public partial class MainWindow
{
    private TitleBar TitleBar()
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

        return titleBar;
    }
}