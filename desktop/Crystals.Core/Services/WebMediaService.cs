using System.Net;
using System.Text;
using System.Text.Json;
using Crystals.Core.Models;

namespace Crystals.Core.Services;

public class WebMediaService(int port)
{
    public event Action<Media>? OnMediaChanged;
    
    public async Task Start()
    {
        var url = $"http://localhost:{port}/";
        using var listener = new HttpListener();
        listener.Prefixes.Add(url);

        try
        {
            listener.Start();
            Console.WriteLine($"[Listener] Service successfully started.");
            Console.WriteLine($"[Listener] Listening for incoming POST requests on {url}...");
            Console.WriteLine($"[Listener] Press Ctrl+C to exit.\n");
            
            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
        }
        catch (HttpListenerException ex)
        {
            Console.WriteLine($"\n[Critical Error] Failed to start HTTP listener: {ex.Message}");
            Console.WriteLine(
                "Note: You may need to run your IDE or console as Administrator to bind to localhost ports.");
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            switch (request.HttpMethod)
            {
                case "OPTIONS":
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Close();
                    return;
                
                case "POST":
                {
                    using var body = request.InputStream;
                    using var reader = new StreamReader(body, request.ContentEncoding);
                    var jsonPayload = reader.ReadToEnd();
                    
                    try
                    {
                        var media = JsonSerializer.Deserialize<Media>(jsonPayload);
                        if (media == null) throw new JsonException("Invalid JSON payload");
                        
                        OnMediaChanged?.Invoke(media);
                    }
                    catch (JsonException)
                    {
                        Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Received raw payload:");
                        Console.WriteLine(jsonPayload);
                    }

                    var buffer = "{\"status\":\"success\"}"u8.ToArray();
                    response.ContentType = "application/json";
                    response.ContentLength64 = buffer.Length;
                    response.StatusCode = (int)HttpStatusCode.OK;

                    using var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    break;
                }
                default:
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Ignored non-POST request: {request.HttpMethod}");
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error] Processing request failed: {ex.Message}");
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            response.Close();
        }
    }
}