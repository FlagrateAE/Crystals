using HidSharp;
using Microsoft.Extensions.Hosting;

namespace Crystals.Core.Services;

public class MysticLightService : BackgroundService
{
    private const int VendorId = 0x1462;
    private const int ProductId = 0x1601;

    public bool IsInitialized;

    private HidDevice? _keyboardDevice;
    private HidStream? _deviceStream;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _keyboardDevice = DeviceList.Local
            .GetHidDevices(VendorId, ProductId)
            .FirstOrDefault();

        if (_keyboardDevice == null)
        {
            throw new Exception("[MysticLightService] Target MSI MysticLight MS-1565 device not found.");
        }

        if (!_keyboardDevice.TryOpen(out _deviceStream))
            throw new Exception("[MysticLightService] Failed to open HID stream. Try running as Administrator.");

        Console.WriteLine(
            $"[MysticLightService] Service successfully started ({_keyboardDevice.GetFriendlyName()})");
        IsInitialized = true;
        return Task.CompletedTask;
    }

    public void SetStaticColor(Ms1565Zone zone, byte r, byte g, byte b)
    {
        // --- PACKET 1: INITIALIZATION ---
        byte[] initBuffer = new byte[65];
        initBuffer[0] = 0x02; // HidSharp Report ID
        initBuffer[1] = 0x01; // Maps to DMS Unknown[0] = 1
        initBuffer[2] = (byte)zone; // Maps to DMS Unknown[1] = 15 (All zones)

        // --- PACKET 2: DATA PAYLOAD ---
        byte[] dataBuffer = new byte[65];
        dataBuffer[0] = 0x02; // HidSharp Report ID
        dataBuffer[1] = 0x02; // Maps to DMS Unknown[0] = 2
        dataBuffer[2] = 0x01; // Maps to DMS Unknown[1] = 1
        dataBuffer[3] = 176; // Maps to DMS Unknown[2] = 176
        dataBuffer[4] = 4; // Maps to DMS Unknown[3] = 4
        dataBuffer[5] = 0; // Maps to DMS Unknown[4] = 0
        dataBuffer[6] = 0; // Maps to DMS Unknown[5] = 0
        dataBuffer[7] = 15; // Maps to DMS Unknown[6] = 15
        dataBuffer[8] = 1; // Maps to DMS Unknown[7] = 1
        dataBuffer[9] = 0; // Maps to DMS Unknown[8] = 0
        dataBuffer[10] = 0; // Maps to DMS Unknown[9] = 0

        // First RGB Block
        dataBuffer[11] = r; // Maps to DMS Unknown[10]
        dataBuffer[12] = g; // Maps to DMS Unknown[11]
        dataBuffer[13] = b; // Maps to DMS Unknown[12]

        dataBuffer[14] = 100; // Maps to DMS Unknown[13] = 100 (Brightness/Alpha)

        // Duplicated Second RGB Block
        dataBuffer[15] = r; // Maps to DMS Unknown[14]
        dataBuffer[16] = g; // Maps to DMS Unknown[15]
        dataBuffer[17] = b; // Maps to DMS Unknown[16]

        // Remaining bytes 18 through 64 are automatically initialized to 0

        _deviceStream!.SetFeature(initBuffer);
        Thread.Sleep(5);
        _deviceStream.SetFeature(dataBuffer);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _deviceStream?.Dispose();
        _deviceStream = null;
        _keyboardDevice = null;
        return base.StopAsync(cancellationToken);
    }
}

public enum Ms1565Zone : byte
{
    Zone1 = 1,
    Zone2 = 2,
    Zone3 = 4,
    Zone4 = 8,
    AllZones = 15
}