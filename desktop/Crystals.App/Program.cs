namespace Crystals.App;

using Windows;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App();
        app.Run(new MainWindow());
    }
}