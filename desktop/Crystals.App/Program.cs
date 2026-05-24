using System.Windows;

namespace Crystals.App;

public class Program
{
    [STAThread]
    public static void Main()
    {
        Application app = new Application();
        app.Run(new MainWindow());
    }
}