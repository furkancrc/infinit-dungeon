namespace InfiniteDungeon.Core;

public class Game
{

    private int roomCount;

    public void StartGame()
    {
        Console.WriteLine("Game started!");
    }

    public void GameLoop()
    {
        while (true)
        {
            // Game loop logic here
            Console.WriteLine("Game loop running...");
            System.Threading.Thread.Sleep(1000); 
        }
    }

    public void NextRoom()
    {
        Console.WriteLine("Moving to the next room...");
        // Logic to move to the next room
    }

    public void GameOver()
    {
        Console.WriteLine("Game over!");
    }

    public void ChooseDoor()
    {

    }
}