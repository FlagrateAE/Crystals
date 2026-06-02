using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Utilities;


namespace Crystals.Core.Devices;

public class MysticLightDevice : IDevice
{
    public string Name => "MSI Mystic Light";

    public BitmapSource Icon { get; } =
        BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Devices/Keyboard.png"));

    private readonly Dictionary<string, string[]> _deviceLedMap = new();

    public bool Start()
    {
        var status = MysticLightNative.Initialize();
        if (status != MLAPI_Status.MLAPI_OK)
        {
            LogNativeError("Initialization Failed", status);
            return false;
        }

        MysticLightNative.GetDeviceInfo(out var deviceTypes, out _);

        foreach (var device in deviceTypes)
        {
            status = MysticLightNative.GetLedName(device, out var ledNames);
            if (status != MLAPI_Status.MLAPI_OK) continue;

            _deviceLedMap[device] = ledNames;
            MysticLightNative.SetLedStyle(device, 0, "Direct All Sync");
        }
        
        return true;
    }

    public void SetColor(CrystalsColor color)
    {
        foreach (var kvp in _deviceLedMap)
        {
            string deviceType = kvp.Key;
            string[] ledNames = kvp.Value;

            int[] rArray = new int[ledNames.Length];
            int[] gArray = new int[ledNames.Length];
            int[] bArray = new int[ledNames.Length];

            var rgb = ColorConverter.HSVtoRGB(color);
            Array.Fill(rArray, rgb.R);
            Array.Fill(gArray, rgb.G);
            Array.Fill(bArray, rgb.B);

            MysticLightNative.SetLedColorsSync(deviceType, ref ledNames, rArray, gArray, bArray);
        }
    }

    public void SetColorSmooth(CrystalsColor color) => SetColor(color);

    private void LogNativeError(string context, MLAPI_Status status)
    {
        if (MysticLightNative.GetErrorMessage((int)status, out string description) == MLAPI_Status.MLAPI_OK)
        {
            Console.WriteLine($"[{context}] Error: {description} ({status})");
        }
    }

    public void Stop()
    {
        MysticLightNative.Release();
    }
}