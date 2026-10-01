namespace InfiniteDungeon.Core;

// Levée quand l'entrée standard est fermée (Ctrl+Z / Ctrl+D ou entrée redirigée vide).
public class EndOfInputException : Exception
{
}

public static class Input
{
    public static string Read(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();
        if (line == null)
            throw new EndOfInputException();

        return line.Trim();
    }

    public static string Choose(string prompt, params string[] valid)
    {
        while (true)
        {
            string answer = Read(prompt).ToLowerInvariant();
            if (valid.Contains(answer))
                return answer;

            Console.WriteLine("Choix invalide.");
        }
    }

    public static int ChooseNumber(string prompt, int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Read(prompt), out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Entrez un nombre entre {min} et {max}.");
        }
    }
}
