using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ReactApp2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RealWeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public RealWeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("odesa")]
        public async Task<IActionResult> GetOdessaForecast()
        {
            try
            {
                var url = "https://api.open-meteo.com/v1/forecast?latitude=46.48&longitude=30.73&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max,wind_speed_10m_max&timezone=Europe/Kyiv&forecast_days=16";

                var data = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);

                if (data == null || data.Daily == null)
                    return StatusCode(503, new { error = "Не вдалося отримати дані від сервісу погоди" });

                var forecasts = new List<WeatherDay>();

                for (int i = 0; i < data.Daily.Time.Length; i++)
                {
                    forecasts.Add(new WeatherDay
                    {
                        Date = DateTime.Parse(data.Daily.Time[i]).ToString("dddd, d MMMM", new System.Globalization.CultureInfo("uk-UA")),
                        TempMax = Math.Round(data.Daily.Temperature2mMax[i]),
                        TempMin = Math.Round(data.Daily.Temperature2mMin[i]),
                        Description = GetUkrainianDescription(data.Daily.WeatherCode[i]),
                        Icon = GetWeatherIcon(data.Daily.WeatherCode[i]),
                        Precipitation = data.Daily.PrecipitationProbabilityMax[i],
                        Wind = Math.Round(data.Daily.WindSpeed10mMax[i])
                    });
                }

                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Помилка сервера: " + ex.Message });
            }
        }

        // моделі:
        private class OpenMeteoResponse
        {
            public DailyData Daily { get; set; } = null!;
        }

        private class DailyData
        {
            [JsonPropertyName("time")]
            public string[] Time { get; set; } = null!;

            [JsonPropertyName("temperature_2m_max")]
            public double[] Temperature2mMax { get; set; } = null!;

            [JsonPropertyName("temperature_2m_min")]
            public double[] Temperature2mMin { get; set; } = null!;

            [JsonPropertyName("weather_code")]
            public int[] WeatherCode { get; set; } = null!;

            [JsonPropertyName("precipitation_probability_max")]
            public int[] PrecipitationProbabilityMax { get; set; } = null!;

            [JsonPropertyName("wind_speed_10m_max")]
            public double[] WindSpeed10mMax { get; set; } = null!;
        }

        public class WeatherDay
        {
            public string Date { get; set; } = "";
            public double TempMax { get; set; }
            public double TempMin { get; set; }
            public string Description { get; set; } = "";
            public string Icon { get; set; } = "";
            public int Precipitation { get; set; }
            public double Wind { get; set; }
        }

        private string GetUkrainianDescription(int code)
        {
            return code switch
            {
                0 => "Ясно",
                1 or 2 => "Мінлива хмарність",
                3 => "Хмарно",
                45 or 48 => "Туман",
                51 or 53 or 55 => "Мряка",
                61 or 63 or 65 => "Дощ",
                71 or 73 or 75 => "Сніг",
                80 or 81 or 82 => "Зливи",
                95 or 96 or 99 => "Гроза",
                _ => "Невідомо"
            };
        }

        private string GetWeatherIcon(int code)
        {
            return code switch
            {
                0 => "☀️",
                1 or 2 => "⛅",
                3 => "☁️",
                45 or 48 => "🌫",
                51 or 53 or 55 or 61 or 63 or 65 or 80 or 81 or 82 => "🌧",
                71 or 73 or 75 => "❄️",
                95 or 96 or 99 => "⛈",
                _ => "🌍"
            };
        }
    }
}