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
            await Task.Delay(TimeSpan.FromSeconds(3));
            return;
        }

        var version = await FetchVersion();
        if (version == null)
        {
            return;
        }

        var versionPath = Path.Combine(localAppData, "Roblox", "Versions", version);
        Console.WriteLine("Versions folder path: {0}", versionPath);

        var clientSettingsPath = CreateDirectoryIfNotExist(Path.Combine(versionPath, "ClientSettings"));

        if (!int.TryParse(GetUserInput("Enter the maximum FPS value: "), out int maxFPS))
        {
            Console.WriteLine("Invalid input");
            return;
        }

        var settings = new Dictionary<string, object> { { "DFIntTaskSchedulerTargetFps", maxFPS } };
        var settingsJSON = JsonConvert.SerializeObject(settings);
        await File.WriteAllTextAsync(Path.Combine(clientSettingsPath, "ClientAppSettings.json"), settingsJSON);

        Console.WriteLine("discord.gg/infernoscripts | Max FPS has been set: {0}", maxFPS);

        await Task.Delay(TimeSpan.FromSeconds(3));
    }

    static async Task<string> FetchVersion()
    {
        using HttpClient client = new();
        var response = await client.GetAsync("https://raw.githubusercontent.com/lxyobaba/version/main/robloxversion");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Version Error. Maybe your roblox is outdated?");
            await Task.Delay(TimeSpan.FromSeconds(3));
            return null;
        }

        var body = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

        return data.TryGetValue("ver", out object verObj) && verObj is string ver ? ver : null;
    }

    static string CreateDirectoryIfNotExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    static string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
