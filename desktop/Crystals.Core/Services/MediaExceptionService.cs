using System.Text.Json;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;
using Microsoft.Extensions.Hosting;

namespace Crystals.Core.Services;

public class MediaExceptionService : BackgroundService
{
    private const string DeveloperName = "Flagrate";
    private const string AppName = "Crystals";
    private const string FileName = "MediaExceptions.json";

    private string _filePath = "";
    private Dictionary<string, CrystalsColor> _data;

    public bool IsInExceptions(Media media, out CrystalsColor specialColor)
    {
        specialColor = CrystalsColor.White;
        return _data.TryGetValue(media.ToString(), out specialColor);
    }

    public void AddException(Media media, CrystalsColor specialColor)
    {
        Console.WriteLine($"[MediaExceptionService] Adding exception for {media}");
        _data[media.ToString()] = specialColor;
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

    private Dictionary<string, CrystalsColor> LoadRecords()
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            if (!File.Exists(_filePath))
            {
                return [];
            }

            var jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, CrystalsColor>>(jsonString, options) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return [];
        }
    }

    private void SaveRecords(Dictionary<string, CrystalsColor> data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
        try
        {
            var jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}