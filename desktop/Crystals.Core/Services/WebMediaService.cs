using System.Drawing;
using System.Net;
using System.Text.Json;
using Crystals.Core.Models;
using Microsoft.Extensions.Hosting;

namespace Crystals.Core.Services;

public class WebMediaService(int port) : BackgroundService
{
    public event Action<Media>? OnMediaChanged;
    public Media? CurrentMedia { get; private set; }

    private readonly HttpClient _httpClient = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var url = $"http://localhost:{port}/";
        using var listener = new HttpListener();
        listener.Prefixes.Add(url);

        try
        {
            listener.Start();
            Console.WriteLine($"[WebMediaService] Service successfully started");

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

    private async Task HandleRequest(HttpListenerContext context)
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
                        var mediaDto = JsonSerializer.Deserialize<WebMediaDto>(jsonPayload);
                        if (mediaDto == null) throw new JsonException("Invalid JSON payload");

                        var media = await CreateFromDto(mediaDto);
                        CurrentMedia = media;
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
            Console.WriteLine($"\n[Error: {ex.GetType()}] Processing request failed: {ex.Message}");
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            response.Close();
        }
    }

    private async Task<Media> CreateFromDto(WebMediaDto dto)
    {
        using var httpResponse = await _httpClient.GetAsync(dto.Thumbnail);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to fetch thumbnail. HTTP status code: {httpResponse.StatusCode}"
            );
        }

        var data = await httpResponse.Content.ReadAsByteArrayAsync();
        using var managedStream = new MemoryStream(data);
        using var bitmap = new Bitmap(managedStream);
        var processedBitmap = new Bitmap(bitmap);
        return new Media(dto.Title, dto.Artist, processedBitmap);
    }

    private record WebMediaDto(string Title, string Artist, string Thumbnail);
}