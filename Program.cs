using System.Data.SqlClient;
using Yandex_Weather_API.Settings;
using Yandex_Weather_API.Classes;

enum cities  {Moscow, SaintPetersburg, Ufa, Kazan, Omsk};

class Program
{
    public static async Task Main(string[] args)
    {
        var settingLoader = new SettingsLoader();
        var settings = settingLoader.Settings;

        using (SqlConnection SQLServerConnection = new SqlConnection(settings.ConnectionString))
        {
            Console.WriteLine("Пробуем подключиться к базе данных...");
            try
            {
                SQLServerConnection.OpenAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Соединение с базой установлено");
                //создаем список населенных пунктов с координатами широты и долготы
                List<Locality> localities = new List<Locality>()
                {
                    new Locality("lat=55.75&lon=37.62", "Москва"),
                    new Locality("lat=59.94&lon=30.31", "Санкт-Петербург"),
                    new Locality("lat=54.74&lon=55.97", "Уфа"),
                    new Locality("lat=55.79&lon=49.12", "Казань"),
                    new Locality("lat=54.99&lon=73.37", "Омск")
                };
                //выбираем населенный пунтк, получаем для него сводку погоды,
                //добавляем запись в базу данных
                Weather weather = new Weather(Locality.SelectLocality(localities), SQLServerConnection);
                await weather.GetForecast(settings.YandexApiKey, settings.MinutesTimeCheck);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}