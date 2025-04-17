using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoLinq.Models
{
    public class WeatherData
    {
        [JsonProperty("dt")]
        public long Timestamp { get; set; }

        [JsonProperty("dt_txt")]
        public required string DateTime { get; set; }

        [JsonProperty("main")]
        public required MainData Main { get; set; }

        [JsonProperty("weather")]
        public required List<WeatherInfo> Weather { get; set; }

        [JsonProperty("clouds")]
        public required CloudData Clouds { get; set; }

        [JsonProperty("wind")]
        public required WindData Wind { get; set; }

        [JsonProperty("visibility")]
        public int Visibility { get; set; }

        [JsonProperty("pop")]
        public double ProbabilityOfPrecipitation { get; set; }

        [JsonProperty("sys")]
        public required SysData Sys { get; set; }

        public System.DateTime GetDateTime()
        {
            return System.DateTime.Parse(DateTime);
        }

        public string GetFormattedDate()
        {
            return GetDateTime().ToString("dd/MM/yyyy");
        }

        public string GetFormattedTime()
        {
            return GetDateTime().ToString("HH:mm");
        }

        public string GetWeatherDescription()
        {
            return Weather?[0]?.Description ?? "Inconnu";
        }
    }

    public class MainData
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }

        [JsonProperty("temp_min")]
        public double TempMin { get; set; }

        [JsonProperty("temp_max")]
        public double TempMax { get; set; }

        [JsonProperty("pressure")]
        public int Pressure { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("main")]
        public required string Main { get; set; }

        [JsonProperty("description")]
        public required string Description { get; set; }

        [JsonProperty("icon")]
        public required string Icon { get; set; }
    }

    public class CloudData
    {
        [JsonProperty("all")]
        public int Cloudiness { get; set; }
    }

    public class WindData
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public int Deg { get; set; }

        [JsonProperty("gust")]
        public double Gust { get; set; }
    }

    public class SysData
    {
        [JsonProperty("pod")]
        public required string Pod { get; set; }
    }
} 