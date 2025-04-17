using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MeteoLinq.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MeteoLinq.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.open-meteo.com/v1/forecast";
        private const double ToulouseLatitude = 43.6047;
        private const double ToulouseLongitude = 1.4442;

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            try
            {
                // Utilisation de l'API open-meteo qui ne nécessite pas de clé API
                string requestUrl = $"{ApiUrl}?latitude={ToulouseLatitude}&longitude={ToulouseLongitude}" +
                                    "&hourly=temperature_2m,relativehumidity_2m,precipitation_probability,weathercode,surface_pressure,windspeed_10m,winddirection_10m,cloudcover" +
                                    "&daily=weathercode,temperature_2m_max,temperature_2m_min,precipitation_probability_max" +
                                    "&current_weather=true&timezone=Europe%2FParis&forecast_days=7";
                
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var openMeteoData = JsonConvert.DeserializeObject<dynamic>(content);
                    
                    // Convertir les données Open-Meteo au format de notre application
                    return ConvertToWeatherForecast(openMeteoData);
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération des données météo: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception lors de la récupération des données météo: {ex.Message}");
                return null;
            }
        }

        private WeatherForecast ConvertToWeatherForecast(dynamic openMeteoData)
        {
            var forecast = new WeatherForecast
            {
                Cod = "200",
                Message = 0,
                Count = 40,
                List = new List<WeatherData>(),
                City = new City
                {
                    Id = 2972315, // ID de Toulouse
                    Name = "Toulouse",
                    Coord = new Coordinates
                    {
                        Latitude = ToulouseLatitude,
                        Longitude = ToulouseLongitude
                    },
                    Country = "FR",
                    Population = 471941,
                    Timezone = 7200,
                    Sunrise = 1600000000, // Valeurs factices
                    Sunset = 1600040000   // Valeurs factices
                }
            };

            // Récupérer les données horaires
            var hourlyTime = openMeteoData.hourly.time;
            var hourlyTemp = openMeteoData.hourly.temperature_2m;
            var hourlyHumidity = openMeteoData.hourly.relativehumidity_2m;
            var hourlyPrecipProb = openMeteoData.hourly.precipitation_probability;
            var hourlyWeatherCode = openMeteoData.hourly.weathercode;
            var hourlyPressure = openMeteoData.hourly.surface_pressure;
            var hourlyWindSpeed = openMeteoData.hourly.windspeed_10m;
            var hourlyWindDirection = openMeteoData.hourly.winddirection_10m;
            var hourlyCloudCover = openMeteoData.hourly.cloudcover;

            int count = Math.Min(hourlyTime.Count, 40); // Limiter à 40 prévisions comme dans l'API OpenWeatherMap

            for (int i = 0; i < count; i++)
            {
                string timeStr = hourlyTime[i].ToString();
                double temp = hourlyTemp[i];
                int humidity = (int)hourlyHumidity[i];
                int precipProb = (int)hourlyPrecipProb[i];
                int weatherCode = (int)hourlyWeatherCode[i];
                double pressure = hourlyPressure[i];
                double windSpeed = hourlyWindSpeed[i];
                int windDirection = (int)hourlyWindDirection[i];
                int cloudCover = (int)hourlyCloudCover[i];

                var weatherData = new WeatherData
                {
                    Timestamp = DateTimeOffset.Parse(timeStr).ToUnixTimeSeconds(),
                    DateTime = timeStr,
                    Main = new MainData
                    {
                        Temperature = temp,
                        FeelsLike = CalculateFeelsLike(temp, humidity, windSpeed),
                        TempMin = temp - 1, // Valeur approximative
                        TempMax = temp + 1, // Valeur approximative
                        Pressure = (int)pressure,
                        Humidity = humidity
                    },
                    Weather = new List<WeatherInfo>
                    {
                        new WeatherInfo
                        {
                            Id = weatherCode,
                            Main = GetWeatherMainFromCode(weatherCode),
                            Description = GetWeatherDescriptionFromCode(weatherCode),
                            Icon = GetWeatherIconFromCode(weatherCode)
                        }
                    },
                    Clouds = new CloudData
                    {
                        Cloudiness = cloudCover
                    },
                    Wind = new WindData
                    {
                        Speed = windSpeed,
                        Deg = windDirection,
                        Gust = windSpeed * 1.2 // Valeur approximative
                    },
                    Visibility = 10000, // Valeur par défaut
                    ProbabilityOfPrecipitation = precipProb / 100.0,
                    Sys = new SysData
                    {
                        Pod = IsDay(timeStr) ? "d" : "n"
                    }
                };

                forecast.List.Add(weatherData);
            }

            return forecast;
        }

        private bool IsDay(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            return time.Hour >= 6 && time.Hour < 20; // Considère qu'il fait jour entre 6h et 20h
        }

        private double CalculateFeelsLike(double temp, int humidity, double windSpeed)
        {
            // Calcul approximatif de la température ressentie
            if (temp > 20)
            {
                // Indice de chaleur pour temps chaud
                return temp + 0.05 * humidity - 0.03 * windSpeed;
            }
            else
            {
                // Refroidissement éolien pour temps frais
                return temp - 0.05 * windSpeed;
            }
        }

        private string GetWeatherMainFromCode(int code)
        {
            // Conversion des codes météo Open-Meteo en catégories principales
            return code switch
            {
                0 => "Clear",
                1 or 2 or 3 => "Clouds",
                45 or 48 => "Fog",
                51 or 53 or 55 => "Drizzle",
                56 or 57 => "Freezing Drizzle",
                61 or 63 or 65 => "Rain",
                66 or 67 => "Freezing Rain",
                71 or 73 or 75 => "Snow",
                77 => "Snow Grains",
                80 or 81 or 82 => "Rain Showers",
                85 or 86 => "Snow Showers",
                95 => "Thunderstorm",
                96 or 99 => "Thunderstorm with Hail",
                _ => "Unknown"
            };
        }

        private string GetWeatherDescriptionFromCode(int code)
        {
            // Description détaillée basée sur les codes météo Open-Meteo
            return code switch
            {
                0 => "Ciel dégagé",
                1 => "Principalement dégagé",
                2 => "Partiellement nuageux",
                3 => "Nuageux",
                45 => "Brouillard",
                48 => "Brouillard givrant",
                51 => "Bruine légère",
                53 => "Bruine modérée",
                55 => "Bruine dense",
                56 => "Bruine verglaçante légère",
                57 => "Bruine verglaçante dense",
                61 => "Pluie légère",
                63 => "Pluie modérée",
                65 => "Pluie forte",
                66 => "Pluie verglaçante légère",
                67 => "Pluie verglaçante forte",
                71 => "Neige légère",
                73 => "Neige modérée",
                75 => "Neige forte",
                77 => "Grains de neige",
                80 => "Averses de pluie légères",
                81 => "Averses de pluie modérées",
                82 => "Averses de pluie violentes",
                85 => "Averses de neige légères",
                86 => "Averses de neige fortes",
                95 => "Orage",
                96 => "Orage avec grêle légère",
                99 => "Orage avec grêle forte",
                _ => "Conditions météo inconnues"
            };
        }

        private string GetWeatherIconFromCode(int code)
        {
            // Conversion des codes météo en icônes similaires à celles d'OpenWeatherMap
            return code switch
            {
                0 => "01d", // Ciel dégagé
                1 => "02d", // Principalement dégagé
                2 => "03d", // Partiellement nuageux
                3 => "04d", // Nuageux
                45 or 48 => "50d", // Brouillard
                51 or 53 or 55 => "09d", // Bruine
                56 or 57 => "09d", // Bruine verglaçante
                61 or 63 or 65 => "10d", // Pluie
                66 or 67 => "10d", // Pluie verglaçante
                71 or 73 or 75 or 77 => "13d", // Neige
                80 or 81 or 82 => "09d", // Averses de pluie
                85 or 86 => "13d", // Averses de neige
                95 or 96 or 99 => "11d", // Orage
                _ => "50d" // Par défaut (brouillard)
            };
        }
    }
} 