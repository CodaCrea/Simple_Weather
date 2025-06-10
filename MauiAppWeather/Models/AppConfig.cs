using System.Text.Json;

namespace MauiAppWeather.Models
{
    class AppConfig
    {
        public string? ApiKey { get; set; }

        public static async Task<AppConfig?> LoadKeyAsync()
        {
            Stream stream = await FileSystem.OpenAppPackageFileAsync("config.json");
            StreamReader reader = new(stream);
            string json = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(json)) return null;

            AppConfig? config = JsonSerializer.Deserialize<AppConfig>(json);
            return config;
        }
    }
}
