using Crystals.Core.Models;
using System.IO.Ports;

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
        SendData($"{color.RGB.R}.{color.RGB.G}.{color.RGB.B}.");
    }

    public void SetColorSmooth(CrystalsColor color)
    {
        SendData($"{color.RGB.R}.{color.RGB.G}.{color.RGB.B}~");
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        Console.WriteLine($"Received");
        string data = _serialPort.ReadExisting();
    }

    private void SendData(string data)
    {
        // Console.WriteLine($"Sending: {data}");
        _serialPort.WriteLine(data);
    }
}