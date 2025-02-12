using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace DAT
{
    public static class SettingsSaver
    {
        public static async Task SaveSettingsAsync(AppSettings settings, string filePath = "settings.json")
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }

}
