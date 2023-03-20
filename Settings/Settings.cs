namespace Yandex_Weather_API.Settings;

//класс с настройками приложения
public class Settings
{
    //строка подключения к БД
    public string ConnectionString { get; private set; }
    //временной промежуток между обновлением информации
    public int MinutesTimeCheck { get; private set; }
    //ключ от Яндекс АПИ из кабинета разработчика
    public string YandexApiKey { get; private set; }

    public Settings(string connectionString, int minutesTimeCheck, string yandexApiKey)
    {
        ConnectionString = connectionString;
        MinutesTimeCheck = minutesTimeCheck;
        YandexApiKey = yandexApiKey;
    }
}