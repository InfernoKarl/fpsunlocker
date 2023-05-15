using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(localAppData))
        {
            Console.WriteLine("APPDATA ENV Variable Missing. Huh, that's funky.");
            return;
        }

        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/lxyobaba/version/main/robloxversion");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Version Error. Maybe your roblox is outdated?");
            return;
        }

        var body = await response.Content.ReadAsStringAsync();

        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

        if (!data.TryGetValue("ver", out object verObj) || !(verObj is string ver))
        {
            Console.WriteLine("Invalid Version");
            return;
        }

        var robloxPath = Path.Combine(localAppData, "Roblox");
        var versionsPath = Path.Combine(robloxPath, "Versions");
        var versionPath = Path.Combine(versionsPath, ver);
        Console.WriteLine("Versions folder path:", versionPath);

        var clientSettingsPath = Path.Combine(versionPath, "ClientSettings");
        if (!Directory.Exists(clientSettingsPath))
        {
            Directory.CreateDirectory(clientSettingsPath);
        }

        Console.Write("Enter the maximum FPS value: ");
        if (!int.TryParse(Console.ReadLine(), out int maxFPS))
        {
            Console.WriteLine("Invalid input");
            return;
        }

        var settings = new Dictionary<string, object>
        {
            { "DFIntTaskSchedulerTargetFps", maxFPS }
        };

        var settingsJSON = JsonConvert.SerializeObject(settings);
        var settingsFilePath = Path.Combine(clientSettingsPath, "ClientAppSettings.json");
        await File.WriteAllTextAsync(settingsFilePath, settingsJSON);

        Console.WriteLine("discord.gg/infernoscripts | Max FPS has been set: ", maxFPS);
    }
}
