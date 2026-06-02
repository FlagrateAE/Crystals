using System.IO.Ports;
using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;

namespace Crystals.Core.Devices;

public class ArduinoDevice(string portName, int baudRate) : IDevice
{
    public string Name => "Arduino Uno R3";

    public BitmapSource Icon { get; } =
        BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Devices/Arduino.png"));

    private const int RwTimeout = 500;
    private SerialPort _serialPort = null!;

    public bool Start()
    {
        _serialPort = new SerialPort(portName, baudRate);

        _serialPort.Parity = Parity.None;
        _serialPort.DataBits = 8;
        _serialPort.StopBits = StopBits.One;
        _serialPort.Handshake = Handshake.None;

        _serialPort.ReadTimeout = RwTimeout;
        _serialPort.WriteTimeout = RwTimeout;

        try
        {
            _serialPort.Open();
            _serialPort.DataReceived += OnDataReceived;

            Console.WriteLine("[ArduinoDevice] Device successfully started");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ArduinoDevice] Couldn't start! {e.Message}");
            return false;
        }
    }

    public void SetColor(CrystalsColor color)
    {
        var rgb = ColorConverter.HSVtoRGB(color);
        SendData($"{rgb.R}.{rgb.G}.{rgb.B}.");
    }

    public void SetColorSmooth(CrystalsColor color)
    {
        var rgb = ColorConverter.HSVtoRGB(color);
        SendData($"{rgb.R}.{rgb.G}.{rgb.B}~");
        Console.WriteLine($"[ArduinoDevice] Set color to {color.ToStringRGB()}");
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data = _serialPort.ReadExisting();
        Console.WriteLine($"Received {data}");
    }

    private void SendData(string data)
    {
        // Console.WriteLine($"Sending: {data}");
        _serialPort.WriteLine(data);
    }

    public void Stop()
    {
        _serialPort.Close();
    }
}