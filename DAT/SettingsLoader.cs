using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DAT.Models;

namespace DAT
{
    public static class SettingsLoader
    {
        public static async Task<AppSettings> LoadSettingsAsync(string filePath = "settings.json")
        {
            if (!File.Exists(filePath))
                return new AppSettings(); // Return defaults if file is missing

            string json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
    }
}
