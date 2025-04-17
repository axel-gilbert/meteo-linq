using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MeteoLinq.Models;
using MeteoLinq.Services;
using MeteoLinq.Utils;

namespace MeteoLinq.UI
{
    public class ConsoleUI
    {
        private readonly WeatherService _weatherService;
        private readonly ExportService _exportService;
        private WeatherForecast? _forecast;
        private List<WeatherData> _filteredData;
        private List<WeatherData> _originalData; // Pour garder une copie des données originales

        public ConsoleUI()
        {
            _weatherService = new WeatherService();
            _exportService = new ExportService();
            _filteredData = new List<WeatherData>();
            _originalData = new List<WeatherData>();
        }

        public async Task RunAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Application Météo pour Toulouse avec LINQ ===");
            
            await LoadDataAsync();
            
            if (_forecast == null || _forecast.List == null || !_forecast.List.Any())
            {
                Console.WriteLine("Impossible de charger les données météorologiques. Fin du programme.");
                return;
            }

            _originalData = _forecast.List.ToList();
            _filteredData = _originalData.ToList();
            
            bool exit = false;
            while (!exit)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        FilterData();
                        break;
                    case "2":
                        SortData();
                        break;
                    case "3":
                        GroupData();
                        break;
                    case "4":
                        DisplayData(_filteredData);
                        break;
                    case "5":
                        ExportData();
                        break;
                    case "6":
                        ResetAllFiltersAndSorts();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Option invalide, veuillez réessayer.");
                        break;
                }
            }
            
            Console.WriteLine("Merci d'avoir utilisé l'application Météo LINQ. Au revoir !");
        }

        private async Task LoadDataAsync()
        {
            Console.WriteLine("Chargement des données météorologiques pour Toulouse...");
            _forecast = await _weatherService.GetWeatherForecastAsync();
            
            if (_forecast != null && _forecast.List != null && _forecast.List.Any())
            {
                Console.WriteLine($"Données chargées avec succès! {_forecast.List.Count} prévisions disponibles.");
            }
            else
            {
                Console.WriteLine("Échec du chargement des données météorologiques.");
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("\n=== Menu Principal ===");
            Console.WriteLine("1. Filtrer les données");
            Console.WriteLine("2. Trier les données");
            Console.WriteLine("3. Grouper les données");
            Console.WriteLine("4. Afficher les données");
            Console.WriteLine("5. Exporter les données");
            Console.WriteLine("6. Réinitialiser tous les filtres et tris");
            Console.WriteLine("0. Quitter");
            Console.Write("Votre choix: ");
        }

        private void FilterData()
        {
            Console.WriteLine("\n=== Filtrage des Données ===");
            Console.WriteLine("1. Par date");
            Console.WriteLine("2. Par plage de dates");
            Console.WriteLine("3. Par plage de températures");
            Console.WriteLine("4. Par condition météorologique");
            Console.WriteLine("5. Réinitialiser les filtres");
            Console.WriteLine("0. Retour");
            Console.Write("Votre choix: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    FilterByDate();
                    break;
                case "2":
                    FilterByDateRange();
                    break;
                case "3":
                    FilterByTemperatureRange();
                    break;
                case "4":
                    FilterByWeatherCondition();
                    break;
                case "5":
                    ResetFilters();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }

        private void FilterByDate()
        {
            Console.Write("Entrez la date (format: JJ/MM/AAAA): ");
            string dateInput = Console.ReadLine();
            
            if (DateTime.TryParseExact(dateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                _filteredData = _filteredData.FilterByDate(date).ToList();
                Console.WriteLine($"Filtrage effectué. {_filteredData.Count} entrées trouvées.");
                
                // Afficher les résultats après filtrage
                DisplayData(_filteredData);
            }
            else
            {
                Console.WriteLine("Format de date invalide. Veuillez utiliser le format JJ/MM/AAAA.");
            }
        }

        private void FilterByDateRange()
        {
            Console.Write("Entrez la date de début (format: JJ/MM/AAAA): ");
            string startDateInput = Console.ReadLine();
            
            Console.Write("Entrez la date de fin (format: JJ/MM/AAAA): ");
            string endDateInput = Console.ReadLine();
            
            if (DateTime.TryParseExact(startDateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) &&
                DateTime.TryParseExact(endDateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                _filteredData = _filteredData.FilterByDateRange(startDate, endDate).ToList();
                Console.WriteLine($"Filtrage effectué. {_filteredData.Count} entrées trouvées.");
                
                // Afficher les résultats après filtrage
                DisplayData(_filteredData);
            }
            else
            {
                Console.WriteLine("Format de date invalide. Veuillez utiliser le format JJ/MM/AAAA.");
            }
        }

        private void FilterByTemperatureRange()
        {
            Console.Write("Entrez la température minimale (°C): ");
            if (!double.TryParse(Console.ReadLine(), out double minTemp))
            {
                Console.WriteLine("Valeur invalide.");
                return;
            }
            
            Console.Write("Entrez la température maximale (°C): ");
            if (!double.TryParse(Console.ReadLine(), out double maxTemp))
            {
                Console.WriteLine("Valeur invalide.");
                return;
            }
            
            _filteredData = _filteredData.FilterByTemperatureRange(minTemp, maxTemp).ToList();
            Console.WriteLine($"Filtrage effectué. {_filteredData.Count} entrées trouvées.");
            
            // Afficher les résultats après filtrage
            DisplayData(_filteredData);
        }

        private void FilterByWeatherCondition()
        {
            Console.Write("Entrez une condition météorologique (ex: pluie, nuage, soleil): ");
            string condition = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(condition))
            {
                _filteredData = _filteredData.FilterByWeatherCondition(condition).ToList();
                Console.WriteLine($"Filtrage effectué. {_filteredData.Count} entrées trouvées.");
                
                // Afficher les résultats après filtrage
                DisplayData(_filteredData);
            }
            else
            {
                Console.WriteLine("Condition invalide.");
            }
        }

        private void ResetFilters()
        {
            _filteredData = _originalData.ToList();
            Console.WriteLine("Filtres réinitialisés. Toutes les données sont disponibles.");
            
            // Afficher les résultats après réinitialisation
            DisplayData(_filteredData);
        }

        private void ResetAllFiltersAndSorts()
        {
            _filteredData = _originalData.ToList();
            Console.WriteLine("Tous les filtres et tris ont été réinitialisés. Affichage des données originales.");
            
            // Afficher les résultats après réinitialisation
            DisplayData(_filteredData);
        }

        private void SortData()
        {
            Console.WriteLine("\n=== Tri des Données ===");
            Console.WriteLine("1. Par température (croissant)");
            Console.WriteLine("2. Par température (décroissant)");
            Console.WriteLine("3. Par date (croissant)");
            Console.WriteLine("4. Par date (décroissant)");
            Console.WriteLine("0. Retour");
            Console.Write("Votre choix: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    _filteredData = _filteredData.SortByTemperatureAscending().ToList();
                    Console.WriteLine("Données triées par température croissante.");
                    // Afficher les résultats après tri
                    DisplayData(_filteredData);
                    break;
                case "2":
                    _filteredData = _filteredData.SortByTemperatureDescending().ToList();
                    Console.WriteLine("Données triées par température décroissante.");
                    // Afficher les résultats après tri
                    DisplayData(_filteredData);
                    break;
                case "3":
                    _filteredData = _filteredData.SortByDateAscending().ToList();
                    Console.WriteLine("Données triées par date croissante.");
                    // Afficher les résultats après tri
                    DisplayData(_filteredData);
                    break;
                case "4":
                    _filteredData = _filteredData.SortByDateDescending().ToList();
                    Console.WriteLine("Données triées par date décroissante.");
                    // Afficher les résultats après tri
                    DisplayData(_filteredData);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }

        private void GroupData()
        {
            Console.WriteLine("\n=== Regroupement des Données ===");
            Console.WriteLine("1. Par date");
            Console.WriteLine("2. Par condition météorologique");
            Console.WriteLine("3. Température moyenne par jour");
            Console.WriteLine("4. Température maximale par jour");
            Console.WriteLine("5. Température minimale par jour");
            Console.WriteLine("0. Retour");
            Console.Write("Votre choix: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    DisplayGroupedByDate();
                    break;
                case "2":
                    DisplayGroupedByWeatherCondition();
                    break;
                case "3":
                    DisplayAverageTemperatureByDate();
                    break;
                case "4":
                    DisplayMaxTemperatureByDate();
                    break;
                case "5":
                    DisplayMinTemperatureByDate();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }

        private void DisplayGroupedByDate()
        {
            var groupedData = _filteredData.GroupByDate();
            
            Console.WriteLine("\n=== Données Groupées par Date ===");
            foreach (var group in groupedData)
            {
                Console.WriteLine($"\nDate: {group.Key} ({group.Count()} prévisions)");
                foreach (var item in group)
                {
                    Console.WriteLine($"  {item.GetFormattedTime()} - {item.Main.Temperature}°C - {item.GetWeatherDescription()}");
                }
            }
        }

        private void DisplayGroupedByWeatherCondition()
        {
            var groupedData = _filteredData.GroupByWeatherCondition();
            
            Console.WriteLine("\n=== Données Groupées par Condition Météorologique ===");
            foreach (var group in groupedData)
            {
                Console.WriteLine($"\nCondition: {group.Key} ({group.Count()} prévisions)");
                foreach (var item in group)
                {
                    Console.WriteLine($"  {item.GetFormattedDate()} {item.GetFormattedTime()} - {item.Main.Temperature}°C");
                }
            }
        }

        private void DisplayAverageTemperatureByDate()
        {
            var averageTemps = _filteredData.GetAverageTemperatureByDate();
            
            Console.WriteLine("\n=== Température Moyenne par Jour ===");
            foreach (var item in averageTemps)
            {
                Console.WriteLine($"Date: {item.Key} - Température moyenne: {item.Value}°C");
            }
        }

        private void DisplayMaxTemperatureByDate()
        {
            var maxTemps = _filteredData.GetMaxTemperatureByDate();
            
            Console.WriteLine("\n=== Température Maximale par Jour ===");
            foreach (var item in maxTemps)
            {
                Console.WriteLine($"Date: {item.Key} - Température maximale: {item.Value}°C");
            }
        }

        private void DisplayMinTemperatureByDate()
        {
            var minTemps = _filteredData.GetMinTemperatureByDate();
            
            Console.WriteLine("\n=== Température Minimale par Jour ===");
            foreach (var item in minTemps)
            {
                Console.WriteLine($"Date: {item.Key} - Température minimale: {item.Value}°C");
            }
        }

        private void DisplayData(List<WeatherData> data, int pageSize = 10)
        {
            if (data == null || !data.Any())
            {
                Console.WriteLine("Aucune donnée à afficher.");
                return;
            }
            
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n=== Affichage des Données (Page {currentPage + 1}/{totalPages}) ===");
                Console.WriteLine("Date       | Heure | Température | Description");
                Console.WriteLine("--------------------------------------------------");
                
                var pageData = data.Skip(currentPage * pageSize).Take(pageSize);
                
                foreach (var item in pageData)
                {
                    Console.WriteLine($"{item.GetFormattedDate()} | {item.GetFormattedTime()} | {item.Main.Temperature,5:F1}°C | {item.GetWeatherDescription()}");
                }
                
                Console.WriteLine("\nActions: [P]récédent, [S]uivant, [R]etour");
                Console.Write("Votre choix: ");
                
                var key = Console.ReadLine()?.ToUpper();
                
                if (key == "P" && currentPage > 0)
                {
                    currentPage--;
                }
                else if (key == "S" && currentPage < totalPages - 1)
                {
                    currentPage++;
                }
                else if (key == "R")
                {
                    break;
                }
            }
        }

        private void ExportData()
        {
            if (_filteredData == null || !_filteredData.Any())
            {
                Console.WriteLine("Aucune donnée à exporter.");
                return;
            }
            
            Console.WriteLine("\n=== Exportation des Données ===");
            Console.WriteLine("1. Exporter en JSON");
            Console.WriteLine("2. Exporter en CSV");
            Console.WriteLine("0. Retour");
            Console.Write("Votre choix: ");
            
            string choice = Console.ReadLine();
            
            if (choice != "1" && choice != "2")
            {
                return;
            }
            
            Console.WriteLine("\nSélectionnez les champs à exporter:");
            Console.WriteLine("1. Tous les champs");
            Console.WriteLine("2. Sélection personnalisée");
            Console.Write("Votre choix: ");
            
            string fieldChoice = Console.ReadLine();
            HashSet<string> fieldsToExport = null;
            
            if (fieldChoice == "2")
            {
                fieldsToExport = SelectFieldsToExport();
            }
            
            Console.Write("\nNom du fichier d'exportation: ");
            string fileName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "meteo_toulouse_export";
            }
            
            if (choice == "1")
            {
                string filePath = $"{fileName}.json";
                _exportService.ExportToJson(_filteredData, filePath, fieldsToExport);
            }
            else
            {
                string filePath = $"{fileName}.csv";
                _exportService.ExportToCsv(_filteredData, filePath, fieldsToExport);
            }
        }

        private HashSet<string> SelectFieldsToExport()
        {
            var availableFields = new Dictionary<int, string>
            {
                { 1, "Date" },
                { 2, "Heure" },
                { 3, "Temperature" },
                { 4, "Ressenti" },
                { 5, "TempMin" },
                { 6, "TempMax" },
                { 7, "Humidite" },
                { 8, "Pression" },
                { 9, "DescriptionMeteo" },
                { 10, "VitesseVent" },
                { 11, "DirectionVent" },
                { 12, "Nuages" },
                { 13, "ProbabilitePrecipitation" }
            };
            
            var selectedFields = new HashSet<string>();
            
            Console.WriteLine("\nChamps disponibles:");
            foreach (var field in availableFields)
            {
                Console.WriteLine($"{field.Key}. {field.Value}");
            }
            
            Console.WriteLine("\nEntrez les numéros des champs à exporter séparés par des virgules:");
            string input = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(input))
            {
                var fieldNumbers = input.Split(',')
                                       .Select(s => s.Trim())
                                       .Where(s => int.TryParse(s, out _))
                                       .Select(int.Parse);
                
                foreach (var number in fieldNumbers)
                {
                    if (availableFields.TryGetValue(number, out string fieldName))
                    {
                        selectedFields.Add(fieldName);
                    }
                }
            }
            
            return selectedFields;
        }
    }
} 