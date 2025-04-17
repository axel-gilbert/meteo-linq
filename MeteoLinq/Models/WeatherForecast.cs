using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoLinq.Models
{
    public class WeatherForecast
    {
        [JsonProperty("cod")]
        public required string Cod { get; set; }

        [JsonProperty("message")]
        public int Message { get; set; }

        [JsonProperty("cnt")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public required List<WeatherData> List { get; set; }

        [JsonProperty("city")]
        public required City City { get; set; }
    }

    public class City
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("coord")]
        public required Coordinates Coord { get; set; }

        [JsonProperty("country")]
        public required string Country { get; set; }

        [JsonProperty("population")]
        public int Population { get; set; }

        [JsonProperty("timezone")]
        public int Timezone { get; set; }

        [JsonProperty("sunrise")]
        public long Sunrise { get; set; }

        [JsonProperty("sunset")]
        public long Sunset { get; set; }
    }

    public class Coordinates
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }
} 