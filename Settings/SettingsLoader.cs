using System.IO;
using Newtonsoft.Json;

namespace Yandex_Weather_API.Settings;

public class SettingsLoader
{
    //путь к файлу application.json
    private string PathToFile = @"/Users/renatnigamadzanov/Documents/Rider Projects/Yandex Weather API/Yandex Weather API/appsettings.json";
    private string DataFile;
        
    public Settings Settings { get; private set; }

    public SettingsLoader()
    {
        DataFile = File.ReadAllText(PathToFile);
        Settings = ParseFile();
    }
    private Settings ParseFile()
    {
        dynamic jsonData = JsonConvert.DeserializeObject(DataFile);
        var result = new Settings((string)jsonData.ConnectionString, (int)jsonData.MinutesTimeCheck, (string)jsonData.YandexAPIKey);
        return result;
    }
}