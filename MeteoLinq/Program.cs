using System;
using System.Threading.Tasks;
using MeteoLinq.UI;

namespace MeteoLinq
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                ConsoleUI ui = new ConsoleUI();
                await ui.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur inattendue s'est produite: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            // Attendre que l'utilisateur appuie sur une touche avant de quitter
            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
    }
}
