using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestWork
{
    class Program
    {
        public const string API_KEY = "c402338c45b2d16ee3125054d1cb874c";
        static async Task Main(string[] args)
        {
            string inputCity = "";
            Console.WriteLine("Введите город: ");
            inputCity =  Console.ReadLine();
            HttpClient cl = new HttpClient();
            try
            {
                //запрашиваем информацию о городе, чтобы получить координаты
                var response = await cl.GetAsync($"http://api.openweathermap.org/geo/1.0/direct?q={inputCity}&limit=1&appid={API_KEY}");
                //проверяем ответ сервера
                if (response.StatusCode.ToString() !="OK") throw new Exception("Произошла ошибка сервера");
                //считываем ответ
                var cityJSON = await response.Content.ReadAsStringAsync();
                //ответ приходит в виде массива, поэтому сериализуем массив и берем первый результат
                JArray cities = JArray.Parse(cityJSON);
                var firstCity = cities.First;
                //проверяем, есть ли найденный   город
                if (firstCity == null) throw new Exception("Проверьте название города");
                //конвертируем объект к типу City
                var city = firstCity.ToObject<City>();
                //запрашиваем погоду в заданном городе
                response = await cl.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lat={city.lat}&lon={city.lon}&appid={API_KEY}&units=metric");
                //считываем ответ
                var weatherJSON = await response.Content.ReadAsStringAsync();
                JObject weatherObject = JObject.Parse(weatherJSON);
                
                var weather = weatherObject.ToObject<WeatherData>();

                DateTime sunrise = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(weather.sys.sunrise + weather.timezone);
                DateTime sunset = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(weather.sys.sunset + weather.timezone);
                string output = $"Температура - {weather.main.temp.ToString()}\n" +
                                    $"Влажность - {weather.main.humidity.ToString()}%\n" +
                                    $"Восход солнца - {sunrise.Hour.ToString("00")}:{sunrise.Minute.ToString("00")}\n" +
                                    $"Закат солнца - {sunset.Hour.ToString("00")}:{sunset.Minute.ToString("00")}\n";
                //создаем папку для файлов
                if (!Directory.Exists("C:\\Weather")) Directory.CreateDirectory("C:\\Weather");
                //открываем файл или создаем его, если его нет
                using(FileStream fs = new FileStream($"C:\\Weather\\{DateTime.Now.Day.ToString("00")}.{DateTime.Now.Month.ToString("00")}.{DateTime.Now.Year.ToString("0000")}.txt", FileMode.OpenOrCreate))
                {
                    //помещаем курсор в конец файла
                    fs.Seek(0, SeekOrigin.End);
                    //записываем данные
                    fs.Write(Encoding.Default.GetBytes(output + "\n"));
                }
                //выводим сообщение в консоль
                Console.WriteLine(output);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
