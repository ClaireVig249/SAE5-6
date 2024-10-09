using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Chemin vers l'exécutable Stockfish sur macOS (à ajuster selon ton installation)
        string stockfishPath = "Stockfish";  // Remplace par ton chemin exact

        // Créer et démarrer le processus Stockfish
        var stockfishProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = stockfishPath,
                RedirectStandardInput = true,   // Redirection de l'entrée pour envoyer des commandes
                RedirectStandardOutput = true,  // Redirection de la sortie pour lire les réponses
                UseShellExecute = false,
                CreateNoWindow = true           // Ne pas afficher de fenêtre
            }
        };

        stockfishProcess.Start();
        StreamWriter inputWriter = stockfishProcess.StandardInput;
        StreamReader outputReader = stockfishProcess.StandardOutput;

        // Envoyer la commande UCI pour vérifier que Stockfish fonctionne
        await inputWriter.WriteLineAsync("uci");
        await inputWriter.FlushAsync();

        // Lire la sortie pour voir si Stockfish répond avec "uciok"
        string output;
        while ((output = await outputReader.ReadLineAsync()) != null)
        {
            Console.WriteLine(output);
            if (output.Contains("uciok"))
            {
                Console.WriteLine("Stockfish est prêt à recevoir des commandes.");
                break;
            }
        }

        // Envoyer une commande pour vérifier si Stockfish est prêt
        await inputWriter.WriteLineAsync("isready");
        await inputWriter.FlushAsync();

        // Lire la réponse à la commande "isready"
        while ((output = await outputReader.ReadLineAsync()) != null)
        {
            Console.WriteLine(output);
            if (output.Contains("readyok"))
            {
                Console.WriteLine("Stockfish est prêt.");
                break;
            }
        }

        // Envoyer une position de départ (position initiale d'une partie d'échecs)
        await inputWriter.WriteLineAsync("position startpos");
        await inputWriter.FlushAsync();

        // Envoyer une commande pour demander le meilleur coup
        await inputWriter.WriteLineAsync("go depth 10");
        await inputWriter.FlushAsync();

        // Lire et afficher la réponse avec le meilleur coup trouvé par Stockfish
        while ((output = await outputReader.ReadLineAsync()) != null)
        {
            Console.WriteLine(output);
            if (output.StartsWith("bestmove"))
            {
                Console.WriteLine("Meilleur coup trouvé : " + output);
                break;
            }
        }

        // Envoyer la commande pour quitter Stockfish proprement
        await inputWriter.WriteLineAsync("quit");
        await inputWriter.FlushAsync();

        stockfishProcess.WaitForExit();
        Console.WriteLine("Stockfish terminé.");
    }
}