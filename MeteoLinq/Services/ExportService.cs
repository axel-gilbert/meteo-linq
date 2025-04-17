using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using MeteoLinq.Models;
using Newtonsoft.Json;

namespace MeteoLinq.Services
{
    public class ExportService
    {
        public void ExportToJson(List<WeatherData> data, string filePath, HashSet<string> fieldsToExport)
        {
            try
            {
                if (fieldsToExport != null && fieldsToExport.Count > 0)
                {
                    var filteredData = data.Select(d => CreateFilteredObject(d, fieldsToExport)).ToList();
                    string jsonContent = JsonConvert.SerializeObject(filteredData, Formatting.Indented);
                    File.WriteAllText(filePath, jsonContent);
                }
                else
                {
                    string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(filePath, jsonContent);
                }
                
                Console.WriteLine($"Données exportées avec succès vers {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exportation au format JSON: {ex.Message}");
            }
        }

        public void ExportToCsv(List<WeatherData> data, string filePath, HashSet<string> fieldsToExport)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, config))
                {
                    if (fieldsToExport != null && fieldsToExport.Count > 0)
                    {
                        // Création d'une liste d'objets anonymes pour l'exportation
                        var records = data.Select(d => 
                        {
                            var obj = new Dictionary<string, object>();
                            if (fieldsToExport.Contains("Date"))
                                obj["Date"] = d.GetFormattedDate();
                            if (fieldsToExport.Contains("Heure"))
                                obj["Heure"] = d.GetFormattedTime();
                            if (fieldsToExport.Contains("Temperature"))
                                obj["Temperature"] = d.Main.Temperature;
                            if (fieldsToExport.Contains("Ressenti"))
                                obj["Ressenti"] = d.Main.FeelsLike;
                            if (fieldsToExport.Contains("TempMin"))
                                obj["TempMin"] = d.Main.TempMin;
                            if (fieldsToExport.Contains("TempMax"))
                                obj["TempMax"] = d.Main.TempMax;
                            if (fieldsToExport.Contains("Humidite"))
                                obj["Humidite"] = d.Main.Humidity;
                            if (fieldsToExport.Contains("Pression"))
                                obj["Pression"] = d.Main.Pressure;
                            if (fieldsToExport.Contains("DescriptionMeteo"))
                                obj["DescriptionMeteo"] = d.GetWeatherDescription();
                            if (fieldsToExport.Contains("VitesseVent"))
                                obj["VitesseVent"] = d.Wind.Speed;
                            if (fieldsToExport.Contains("DirectionVent"))
                                obj["DirectionVent"] = d.Wind.Deg;
                            if (fieldsToExport.Contains("Nuages"))
                                obj["Nuages"] = d.Clouds.Cloudiness;
                            if (fieldsToExport.Contains("ProbabilitePrecipitation"))
                                obj["ProbabilitePrecipitation"] = d.ProbabilityOfPrecipitation;
                            return obj;
                        }).ToList();

                        // Écrire les en-têtes
                        foreach (var header in fieldsToExport)
                        {
                            csv.WriteField(header);
                        }
                        csv.NextRecord();

                        // Écrire les données
                        foreach (var record in records)
                        {
                            foreach (var field in fieldsToExport)
                            {
                                if (record.TryGetValue(field, out var value))
                                    csv.WriteField(value);
                                else
                                    csv.WriteField(string.Empty);
                            }
                            csv.NextRecord();
                        }
                    }
                    else
                    {
                        // Création d'objets anonymes pour simplifier l'exportation CSV
                        var records = data.Select(d => new
                        {
                            Date = d.GetFormattedDate(),
                            Heure = d.GetFormattedTime(),
                            Temperature = d.Main.Temperature,
                            Ressenti = d.Main.FeelsLike,
                            TempMin = d.Main.TempMin,
                            TempMax = d.Main.TempMax,
                            Humidite = d.Main.Humidity,
                            Pression = d.Main.Pressure,
                            DescriptionMeteo = d.GetWeatherDescription(),
                            VitesseVent = d.Wind.Speed,
                            DirectionVent = d.Wind.Deg,
                            Nuages = d.Clouds.Cloudiness,
                            ProbabilitePrecipitation = d.ProbabilityOfPrecipitation
                        }).ToList();
                        
                        csv.WriteRecords(records);
                    }
                }
                
                Console.WriteLine($"Données exportées avec succès vers {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exportation au format CSV: {ex.Message}");
            }
        }

        private object CreateFilteredObject(WeatherData data, HashSet<string> fieldsToExport)
        {
            var result = new Dictionary<string, object>();

            // Ajouter les champs demandés
            if (fieldsToExport.Contains("Date"))
                result["Date"] = data.GetFormattedDate();
            
            if (fieldsToExport.Contains("Heure"))
                result["Heure"] = data.GetFormattedTime();
            
            if (fieldsToExport.Contains("Temperature"))
                result["Temperature"] = data.Main.Temperature;
            
            if (fieldsToExport.Contains("Ressenti"))
                result["Ressenti"] = data.Main.FeelsLike;
            
            if (fieldsToExport.Contains("TempMin"))
                result["TempMin"] = data.Main.TempMin;
            
            if (fieldsToExport.Contains("TempMax"))
                result["TempMax"] = data.Main.TempMax;
            
            if (fieldsToExport.Contains("Humidite"))
                result["Humidite"] = data.Main.Humidity;
            
            if (fieldsToExport.Contains("Pression"))
                result["Pression"] = data.Main.Pressure;
            
            if (fieldsToExport.Contains("DescriptionMeteo"))
                result["DescriptionMeteo"] = data.GetWeatherDescription();
            
            if (fieldsToExport.Contains("VitesseVent"))
                result["VitesseVent"] = data.Wind.Speed;
            
            if (fieldsToExport.Contains("DirectionVent"))
                result["DirectionVent"] = data.Wind.Deg;
            
            if (fieldsToExport.Contains("Nuages"))
                result["Nuages"] = data.Clouds.Cloudiness;
            
            if (fieldsToExport.Contains("ProbabilitePrecipitation"))
                result["ProbabilitePrecipitation"] = data.ProbabilityOfPrecipitation;

            return result;
        }
    }
} 