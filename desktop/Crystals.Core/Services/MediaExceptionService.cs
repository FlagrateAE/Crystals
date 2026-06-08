using System.Text.Json;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;
using Microsoft.Extensions.Hosting;
using WinRT.Interop;

namespace Crystals.Core.Services;

public class MediaExceptionService : BackgroundService
{
    private record MediaException(string Name, string Description, CrystalsColor SpecialColor);

    private const string DeveloperName = "Flagrate";
    private const string AppName = "Crystals";
    private const string FileName = "MediaExceptions.json";

    private string _filePath = "";
    private List<MediaException> _data;

    public bool IsInExceptions(Media media, out CrystalsColor specialColor)
    {
        var record = _data.FirstOrDefault(e => e.Name == media.Name && e.Description == media.Description);
        specialColor = record?.SpecialColor ?? CrystalsColor.White;
        return record != null;
    }

    public void AddException(Media media, CrystalsColor specialColor)
    {
        Console.WriteLine($"[MediaExceptionService] Adding exception for {media.Name} by {media.Description}");
        _data.Add(new MediaException(media.Name, media.Description, specialColor));
        SaveRecords(_data);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, DeveloperName, AppName);

        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _filePath = Path.Combine(appFolder, FileName);

        _data = LoadRecords();

        Console.WriteLine($"[MediaExceptionService] Service successfully started ({_data.Count} media exceptions)");
        return Task.CompletedTask;
    }

    private List<MediaException> LoadRecords()
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            if (!File.Exists(_filePath))
            {
                return [];
            }

            var jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<MediaException>>(jsonString, options) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return [];
        }
    }

    private void SaveRecords(List<MediaException> records)
    {
        var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
        try
        {
            var jsonString = JsonSerializer.Serialize(records, options);
            File.WriteAllText(_filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}