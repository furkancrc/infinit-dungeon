using System.Text;
using InfiniteDungeon.Core;

namespace InfiniteDungeon;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }
        catch (IOException)
        {
            // pas de console attachée : on garde l'encodage par défaut
        }

        try
        {
            string? seedArgument = args.Length > 0 ? args[0] : null;
            do
            {
                PlayOnce(seedArgument);
                seedArgument = null;
            }
            while (Input.Choose("Rejouer ? [o/n] > ", "o", "n") == "o");
        }
        catch (EndOfInputException)
        {
            Console.WriteLine();
        }
    }

    static void PlayOnce(string? seedArgument)
    {
        string name = Input.Read("Nom du héros > ");
        if (name.Length == 0)
            name = "Héros";

        string seedText = seedArgument ?? Input.Read("Seed (vide = aléatoire) > ");
        int seed = ParseSeed(seedText);

        new Game(name, seed).StartGame();
    }

    // Un nombre est utilisé tel quel, un texte est transformé en nombre de façon stable.
    static int ParseSeed(string text)
    {
        if (text.Length == 0)
            return Random.Shared.Next(100000, 1000000);

        if (int.TryParse(text, out int number))
            return number;

        unchecked
        {
            uint hash = 2166136261;
            foreach (char c in text)
                hash = (hash ^ c) * 16777619;
            return (int)(hash & 0x7FFFFFFF);
        }
    }
}
