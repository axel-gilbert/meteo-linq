using System;
using System.Collections.Generic;
using System.Linq;
using MeteoLinq.Models;

namespace MeteoLinq.Utils
{
    public static class LinqHelpers
    {
        public static IEnumerable<WeatherData> FilterByDate(this IEnumerable<WeatherData> data, DateTime date)
        {
            return data.Where(d => d.GetDateTime().Date == date.Date);
        }

        public static IEnumerable<WeatherData> FilterByDateRange(this IEnumerable<WeatherData> data, DateTime startDate, DateTime endDate)
        {
            return data.Where(d => d.GetDateTime().Date >= startDate.Date && d.GetDateTime().Date <= endDate.Date);
        }

        public static IEnumerable<WeatherData> FilterByTemperatureRange(this IEnumerable<WeatherData> data, double minTemp, double maxTemp)
        {
            return data.Where(d => d.Main.Temperature >= minTemp && d.Main.Temperature <= maxTemp);
        }

        public static IEnumerable<WeatherData> FilterByWeatherCondition(this IEnumerable<WeatherData> data, string condition)
        {
            return data.Where(d => d.Weather.Any(w => w.Main.ToLower().Contains(condition.ToLower()) || 
                                              w.Description.ToLower().Contains(condition.ToLower())));
        }

        public static IEnumerable<WeatherData> SortByTemperatureAscending(this IEnumerable<WeatherData> data)
        {
            return data.OrderBy(d => d.Main.Temperature);
        }

        public static IEnumerable<WeatherData> SortByTemperatureDescending(this IEnumerable<WeatherData> data)
        {
            return data.OrderByDescending(d => d.Main.Temperature);
        }

        public static IEnumerable<WeatherData> SortByDateAscending(this IEnumerable<WeatherData> data)
        {
            return data.OrderBy(d => d.GetDateTime());
        }

        public static IEnumerable<WeatherData> SortByDateDescending(this IEnumerable<WeatherData> data)
        {
            return data.OrderByDescending(d => d.GetDateTime());
        }

        public static IEnumerable<IGrouping<string, WeatherData>> GroupByDate(this IEnumerable<WeatherData> data)
        {
            return data.GroupBy(d => d.GetFormattedDate());
        }

        public static IEnumerable<IGrouping<string, WeatherData>> GroupByWeatherCondition(this IEnumerable<WeatherData> data)
        {
            return data.GroupBy(d => d.GetWeatherDescription());
        }

        public static Dictionary<string, double> GetAverageTemperatureByDate(this IEnumerable<WeatherData> data)
        {
            return data.GroupBy(d => d.GetFormattedDate())
                       .ToDictionary(
                           g => g.Key,
                           g => Math.Round(g.Average(d => d.Main.Temperature), 1)
                       );
        }

        public static Dictionary<string, double> GetMaxTemperatureByDate(this IEnumerable<WeatherData> data)
        {
            return data.GroupBy(d => d.GetFormattedDate())
                       .ToDictionary(
                           g => g.Key,
                           g => Math.Round(g.Max(d => d.Main.Temperature), 1)
                       );
        }

        public static Dictionary<string, double> GetMinTemperatureByDate(this IEnumerable<WeatherData> data)
        {
            return data.GroupBy(d => d.GetFormattedDate())
                       .ToDictionary(
                           g => g.Key,
                           g => Math.Round(g.Min(d => d.Main.Temperature), 1)
                       );
        }
    }
} 