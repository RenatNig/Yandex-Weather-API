using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Yandex_Weather_API.Classes;

//класс информации о погоде
public class Weather
{
    //населенный пункт, для которого узнаем сводку
    private Locality Locality;
    //подключение к БД
    private SqlConnection Connection;

    public Weather(Locality locality, SqlConnection connection)
    {
        Locality = locality;
        Connection = connection;
    }

    //метод вывода погодной сводки в консоль, вызова метода для записи в БД
    public async Task GetForecast(string yandexApiKey, int minutesTimeCheck)
    {
        while (true)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                    "https://api.weather.yandex.ru/v2/forecast?" + Locality.Coordinates + "&lang=ru_RU&extra=false");

                request.Headers.Add("X-Yandex-API-Key", yandexApiKey);

                using HttpResponseMessage response = await httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(content);
                dynamic factSection = result.fact;
                
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("В населенном пункте {0} сейчас:", Locality.Name);
                Console.WriteLine("Температура: {0}°C (ощущается как {1}°C)", factSection.temp, factSection.feels_like);
                Console.WriteLine("Влажность воздуха: {0}%", factSection.humidity);
                Console.WriteLine("Направление ветра: {0}", factSection.wind_dir);
                Console.WriteLine("Скорость ветра: {0} м/с", factSection.wind_speed);
                Console.WriteLine("Скорость порывов ветра: {0} м/с", factSection.wind_gust);
                
                await InsertIntoDatabase(factSection.temp.ToString(), factSection.feels_like.ToString(), (int)factSection.humidity, factSection.wind_dir.ToString(), (double)factSection.wind_speed, (double)factSection.wind_gust);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Обновим сводку через {0} мин.", minutesTimeCheck);

                var task = Task.Delay(minutesTimeCheck * 60000);
                
                while (!task.IsCompleted)
                {
                    Console.WriteLine("Ожидание... Введите q для выхода, Enter для продолжения");
                    string input = Console.ReadLine();
                    if (input == "q")
                        Environment.Exit(0);
                }
            }
        }
    }

    //метод для записи в БД (SQL Server)
    private async Task InsertIntoDatabase(string temperature, string temp_feels, int humidity, string wind_dir, double wind_speed, double wind_gust)
    {
        Console.WriteLine("Делаем запись в базу данных...");
        //запрос
        string sqlExpression = "INSERT INTO MainDb.dbo.WeatherData " +
                               "(DateInsert, Locality, Temperature, Temp_Feels, Humidity, Wind_Direction, Wind_Speed, Wind_Gust) " +
                               "VALUES (@date_insert, @locality, @temperature, @temp_feels, @humidity, @wind_dir, @wind_speed, @wind_gust)";
        
        SqlCommand command = new SqlCommand(sqlExpression, Connection);
        
        //используем параметры для защиты от инъекций
        SqlParameter dateTimeParam = new SqlParameter("@date_insert", DateTime.Now);
        command.Parameters.Add(dateTimeParam);
        SqlParameter localityParam = new SqlParameter("@locality", Locality.Name);
        command.Parameters.Add(localityParam);
        SqlParameter temperatureParam = new SqlParameter("@temperature", temperature);
        command.Parameters.Add(temperatureParam);
        SqlParameter tempFeelsParam = new SqlParameter("@temp_feels", temp_feels);
        command.Parameters.Add(tempFeelsParam);
        SqlParameter humidityParam = new SqlParameter("@humidity", humidity);
        command.Parameters.Add(humidityParam);
        SqlParameter windDirParam = new SqlParameter("@wind_dir", wind_dir);
        command.Parameters.Add(windDirParam);
        SqlParameter windSpeedParam = new SqlParameter("@wind_speed", wind_speed);
        command.Parameters.Add(windSpeedParam);
        SqlParameter windGustParam = new SqlParameter("@wind_gust", wind_gust);
        command.Parameters.Add(windGustParam);
        
        int number = await command.ExecuteNonQueryAsync();
        if (number == 1)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Запись в базу данных добавлена");
        }
    }
}