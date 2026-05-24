using Crystals.Core.Models;
using System.IO.Ports;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;

namespace Crystals.Core.Devices;

public class ArduinoDevice(string portName, int baudRate) : IDevice
{
    private const int RwTimeout = 500;

    private SerialPort _serialPort = null!;

    public void Start()
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
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't open serial port {portName}: {e.Message}");
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