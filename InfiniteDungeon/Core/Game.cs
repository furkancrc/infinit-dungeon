using InfiniteDungeon.Entity;
using InfiniteDungeon.Items;
using InfiniteDungeon.Rooms;

namespace InfiniteDungeon.Core;

public class Game
{
    private const int DoorCount = 3;

    private readonly Player player;
    private readonly Random rng;      // aléa des combats, pièges, fuites...
    private readonly int seed;

    private Room currentRoom;
    private Room[] doors = Array.Empty<Room>();
    private int selectedDoor;
    private int roomCount;
    private bool isRunning;
    private int kills;
    private int goldEarned;

    public int Seed => seed;

    public Game(string playerName, int seed)
    {
        this.seed = seed;
        player = new Player(playerName);
        rng = new Random(Mix(seed, 0, 0));
        currentRoom = new Room(0, RoomType.Start, RoomRng(0, 0));
    }

    // La même seed et les mêmes choix donnent toujours le même donjon.
    private Random RoomRng(int depth, int door) => new Random(Mix(seed, depth, door));

    private static int Mix(int a, int b, int c)
    {
        unchecked
        {
            uint h = (uint)a;
            h = (h * 2654435761u) ^ (uint)b;
            h = ((h ^ (h >> 15)) * 2246822519u) ^ (uint)c;
            h ^= h >> 13;
            h *= 3266489917u;
            h ^= h >> 16;
            return (int)h;
        }
    }

    public void StartGame()
    {
        isRunning = true;
        Console.WriteLine();
        Console.WriteLine("=== INFINITE DUNGEON ===");
        Console.WriteLine($"Bienvenue, {player.Name} ! Seed de la partie : {seed}");
        Console.WriteLine("Descendez le plus profond possible. Un boss vous attend toutes les 10 salles.");
        GameLoop();
    }

    public void GameLoop()
    {
        while (isRunning)
        {
            ChooseDoor();
            if (!isRunning)
                break;

            NextRoom();
            ResolveRoom();

            if (!player.IsAlive)
                isRunning = false;
        }

        GameOver();
    }

    public void ChooseDoor()
    {
        PrepareDoors();

        currentRoom.Display();
        ShowDoors();

        while (true)
        {
            Console.WriteLine("[1-3] choisir une porte  [i] inventaire  [s] statistiques  [q] quitter");
            string choice = Input.Choose("> ", "1", "2", "3", "i", "s", "q");

            switch (choice)
            {
                case "i":
                    ShowInventory();
                    ShowDoors();
                    break;
                case "s":
                    ShowStats();
                    ShowDoors();
                    break;
                case "q":
                    if (Input.Choose("Quitter la partie ? [o/n] > ", "o", "n") == "o")
                    {
                        isRunning = false;
                        return;
                    }
                    break;
                default:
                    selectedDoor = int.Parse(choice) - 1;
                    return;
            }
        }
    }

    private void ShowDoors()
    {
        ShowStatus();
        Console.WriteLine("Que se cache-t-il derrière chaque porte ?");
        for (int i = 0; i < doors.Length; i++)
            Console.WriteLine($"  Porte {i + 1} : {doors[i].Hint}");
    }

    // Génère les 3 salles possibles : elles ne dépendent que de la seed, de la profondeur et de la porte.
    private void PrepareDoors()
    {
        int next = roomCount + 1;
        doors = new Room[DoorCount];
        for (int i = 0; i < DoorCount; i++)
            doors[i] = Room.Create(next, RoomRng(next, i + 1));
    }

    public void NextRoom()
    {
        currentRoom = doors[selectedDoor];
        roomCount = currentRoom.Depth;
        currentRoom.Display();
    }

    private void ResolveRoom()
    {
        switch (currentRoom.Type)
        {
            case RoomType.Combat:
            case RoomType.Boss:
                StartCombat(currentRoom.Enemy!);
                break;
            case RoomType.Merchant:
                OpenShop(currentRoom.Merchant!);
                break;
            case RoomType.Treasure:
                OpenTreasure();
                break;
            case RoomType.Rest:
                Rest();
                break;
            case RoomType.Trap:
                SpringTrap();
                break;
        }

        if (player.IsAlive)
            currentRoom.MarkCleared();
    }

    // ---------- Combat ----------

    public void StartCombat(Enemy enemy)
    {
        Console.WriteLine();
        Console.WriteLine(enemy.IsBoss
            ? $"BOSS ! {enemy.Name} se dresse devant vous !"
            : $"Un {enemy.Name} vous attaque !");

        while (player.IsAlive && enemy.IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine($"{player.Name} : {player.Health}/{player.MaxHealth} PV   |   {enemy.Name} : {enemy.Health}/{enemy.MaxHealth} PV");

            string action;
            if (enemy.IsBoss)
            {
                Console.WriteLine("[1] attaquer  [2] potion");
                action = Input.Choose("> ", "1", "2");
            }
            else
            {
                Console.WriteLine("[1] attaquer  [2] potion  [3] fuir");
                action = Input.Choose("> ", "1", "2", "3");
            }

            if (action == "1")
            {
                AttackResult hit = player.Attack(enemy, rng);
                Console.WriteLine(hit.Critical
                    ? $"Coup critique ! Vous infligez {hit.Damage} dégâts."
                    : $"Vous infligez {hit.Damage} dégâts.");
            }
            else if (action == "2")
            {
                if (!UsePotionInCombat())
                    continue;   // pas de tour perdu si aucune potion n'a été bue
            }
            else if (rng.Next(100) < 50)
            {
                Console.WriteLine("Vous prenez la fuite !");
                return;
            }
            else
            {
                Console.WriteLine("Impossible de fuir !");
            }

            if (!enemy.IsAlive)
                break;

            AttackResult counter = enemy.Attack(player, rng);
            Console.WriteLine($"{enemy.Name} vous inflige {counter.Damage} dégâts.");
        }

        if (!player.IsAlive)
            return;

        kills++;
        goldEarned += enemy.GoldReward;
        player.AddMoney(enemy.GoldReward);

        Console.WriteLine();
        Console.WriteLine($"{enemy.Name} est vaincu ! +{enemy.GoldReward} or, +{enemy.XpReward} XP.");
        if (player.GainXp(enemy.XpReward))
            Console.WriteLine($"NIVEAU {player.Level} ! PV max {player.MaxHealth}, attaque {player.AttackPower}.");

        foreach (Item item in enemy.DropLoot())
        {
            player.AddItem(item);
            Console.WriteLine($"Butin : {item}");
        }
    }

    // Retourne true si une potion a été bue.
    private bool UsePotionInCombat()
    {
        var potions = player.Inventory.OfType<HealPotion>().ToList();
        if (potions.Count == 0)
        {
            Console.WriteLine("Vous n'avez aucune potion.");
            return false;
        }

        if (player.Health >= player.MaxHealth)
        {
            Console.WriteLine("Vos PV sont déjà au maximum.");
            return false;
        }

        for (int i = 0; i < potions.Count; i++)
            Console.WriteLine($"  {i + 1}. {potions[i]}");

        int choice = Input.ChooseNumber("Potion (0 = annuler) > ", 0, potions.Count);
        if (choice == 0)
            return false;

        player.UseItem(potions[choice - 1]);
        return true;
    }

    // ---------- Marchand ----------

    public void OpenShop(Merchant merchant)
    {
        Console.WriteLine();
        Console.WriteLine($"{merchant.Name} : « Bienvenue, voyageur ! »");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Vous avez {player.Money} or.  [1] acheter  [2] vendre  [0] partir");
            string choice = Input.Choose("> ", "0", "1", "2");

            if (choice == "0")
            {
                Console.WriteLine($"{merchant.Name} : « À bientôt ! »");
                return;
            }

            if (choice == "1")
                BuyFrom(merchant);
            else
                SellTo(merchant);
        }
    }

    private void BuyFrom(Merchant merchant)
    {
        if (merchant.Inventory.Count == 0)
        {
            Console.WriteLine("Le marchand n'a plus rien à vendre.");
            return;
        }

        merchant.ShowInventory();
        int choice = Input.ChooseNumber("Acheter (0 = retour) > ", 0, merchant.Inventory.Count);
        if (choice == 0)
            return;

        Item item = merchant.Inventory[choice - 1];
        if (merchant.Sell(item, player))
            Console.WriteLine($"Vous achetez {item.Name} pour {item.Price} or.");
        else
            Console.WriteLine("Vous n'avez pas assez d'or.");
    }

    private void SellTo(Merchant merchant)
    {
        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("Vous n'avez rien à vendre.");
            return;
        }

        for (int i = 0; i < player.Inventory.Count; i++)
        {
            Item item = player.Inventory[i];
            Console.WriteLine($"  {i + 1}. {item}{(player.IsEquipped(item) ? " (équipé)" : "")} - rachat {merchant.BuyPrice(item)} or");
        }

        int choice = Input.ChooseNumber("Vendre (0 = retour) > ", 0, player.Inventory.Count);
        if (choice == 0)
            return;

        Item sold = player.Inventory[choice - 1];
        int price = merchant.BuyPrice(sold);
        if (merchant.Buy(sold, player))
            Console.WriteLine($"Vous vendez {sold.Name} pour {price} or.");
    }

    // ---------- Autres salles ----------

    private void OpenTreasure()
    {
        Console.WriteLine();
        Console.WriteLine("Vous ouvrez un coffre !");
        player.AddMoney(currentRoom.TreasureGold);
        goldEarned += currentRoom.TreasureGold;
        Console.WriteLine($"+{currentRoom.TreasureGold} or.");

        if (currentRoom.TreasureItem is { } item)
        {
            player.AddItem(item);
            Console.WriteLine($"Trouvé : {item}");
        }
    }

    private void Rest()
    {
        int healed = player.Heal(player.MaxHealth * 2 / 5);
        Console.WriteLine();
        Console.WriteLine($"Vous vous reposez près du feu : +{healed} PV ({player.Health}/{player.MaxHealth}).");
    }

    private void SpringTrap()
    {
        Console.WriteLine();
        Console.WriteLine("Un piège ! [1] tenter de le désamorcer (60 %)  [2] le contourner (dégâts réduits de moitié)");
        string choice = Input.Choose("> ", "1", "2");

        if (choice == "1")
        {
            if (rng.Next(100) < 60)
            {
                int bonus = 10 + currentRoom.Depth * 2;
                player.AddMoney(bonus);
                goldEarned += bonus;
                Console.WriteLine($"Piège désamorcé ! Vous trouvez {bonus} or dans le mécanisme.");
                return;
            }

            int damage = player.TakeDamage(currentRoom.TrapDamage);
            Console.WriteLine($"Raté ! Le piège se déclenche : -{damage} PV.");
        }
        else
        {
            int damage = player.TakeDamage(currentRoom.TrapDamage / 2);
            Console.WriteLine($"Vous passez de justesse : -{damage} PV.");
        }
    }

    // ---------- Menus ----------

    private void ShowStatus()
    {
        Console.WriteLine($"Salle {roomCount} | PV {player.Health}/{player.MaxHealth} | Or {player.Money} | Niv {player.Level} | Seed {seed}");
    }

    private void ShowStats()
    {
        Console.WriteLine();
        Console.WriteLine($"--- {player.Name} ---");
        Console.WriteLine($"Niveau {player.Level} ({player.Xp}/{player.XpToNext} XP)");
        Console.WriteLine($"PV {player.Health}/{player.MaxHealth}");
        Console.WriteLine($"Attaque {player.AttackPower} | Défense {player.Defense}");
        Console.WriteLine($"Arme : {(player.EquippedWeapon?.ToString() ?? "aucune")}");
        Console.WriteLine($"Armure : {(player.EquippedArmor?.ToString() ?? "aucune")}");
        Console.WriteLine($"Or {player.Money} | Ennemis vaincus {kills} | Salle {roomCount}");
    }

    private void ShowInventory()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Inventaire ({player.Money} or) ---");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("  (vide)");
                return;
            }

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Item item = player.Inventory[i];
                Console.WriteLine($"  {i + 1}. {item}{(player.IsEquipped(item) ? " (équipé)" : "")}");
            }

            int choice = Input.ChooseNumber("Utiliser / équiper (0 = retour) > ", 0, player.Inventory.Count);
            if (choice == 0)
                return;

            player.UseItem(player.Inventory[choice - 1]);
        }
    }

    public void GameOver()
    {
        isRunning = false;
        Console.WriteLine();
        Console.WriteLine(player.IsAlive ? "Vous quittez le donjon..." : "Vous êtes mort...");
        Console.WriteLine("=== GAME OVER ===");
        Console.WriteLine($"{player.Name} - niveau {player.Level}");
        Console.WriteLine($"Salles atteintes : {roomCount}");
        Console.WriteLine($"Ennemis vaincus : {kills}");
        Console.WriteLine($"Or gagné : {goldEarned}");
        Console.WriteLine($"Pour rejouer exactement ce donjon, utilisez la seed : {seed}");
    }
}
