using InfiniteDungeon.Entity;
using InfiniteDungeon.World;    

namespace InfiniteDungeon.Core;

public class Game
{
    private Player player;
    private room currentRoom;
    private int roomCount;
    private bool isRunning;

    public Game()
    {
        player = new Player();
        roomCount = 0;
    }

    public void StartGame()
    {
        isRunning = true;
        while (isRunning)
        {
            GameLoop();
        }
        NextRoom();
    }

    public void GameLoop()
    {
        while(isRunning)
        {
            ChooseDoor();
        }
    }

    public void nextRoom()
    {
        roomCount++;
        currentRoom = new Room();
        Console.WriteLine($"Room {roomCount}");
    }

    public void OpenShop()
    {
        Console.WriteLine("Shop Ouvert!");
    }

    public void GameOver()
    {
        isRunning = false;
        Console.WriteLine("Game Over!");
    }

    public void ChooseDoor()
    {
        Console.WriteLine("Choississez une porte (1, 2, 3) ou tapez 'shop' pour ouvrir la boutique :");
        string input = Console.ReadLine();
        switch(input)
        {
            case "1":
                Console.WriteLine("Vous avez choisi la porte 1.");
                break;
            case "2":
                Console.WriteLine("Vous avez choisi la porte 2.");
                break;
            case "3":
                Console.WriteLine("Vous avez choisi la porte 3.");
                break;
            case "shop":
                OpenShop();
                break;
            default:
                Console.WriteLine("Entrée invalide. Veuillez choisir une porte (1, 2, 3) ou tapez 'shop' pour ouvrir la boutique.");
                break;
        }
    }
}