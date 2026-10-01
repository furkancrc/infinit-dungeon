using InfiniteDungeon.Entity;
using InfiniteDungeon.Factories;
using InfiniteDungeon.Items;

namespace InfiniteDungeon.Rooms;

public enum RoomType
{
    Start,
    Combat,
    Merchant,
    Treasure,
    Rest,
    Trap,
    Boss
}

public class Room
{
    public const int Height = 11;   // impairs : le centre est exact
    public const int Width = 21;

    private static readonly string[] MerchantNames =
        { "Gorm le colporteur", "Mirabelle", "Old Bartho", "Zelda la rôdeuse", "Fenwick" };

    private readonly char[,] map = new char[Height, Width];

    public int Depth { get; }
    public RoomType Type { get; }
    public Enemy? Enemy { get; }
    public Merchant? Merchant { get; }
    public int TreasureGold { get; }
    public Item? TreasureItem { get; }
    public int TrapDamage { get; }
    public bool Cleared { get; private set; }

    public Room(int depth, RoomType type, Random rng)
    {
        Depth = depth;
        Type = type;

        switch (type)
        {
            case RoomType.Combat:
                Enemy = EnemyFactory.Create(depth, rng);
                break;
            case RoomType.Boss:
                Enemy = EnemyFactory.CreateBoss(depth, rng);
                break;
            case RoomType.Merchant:
                Merchant = new Merchant(MerchantNames[rng.Next(MerchantNames.Length)],
                                        ItemFactory.CreateShopStock(depth, rng));
                break;
            case RoomType.Treasure:
                TreasureGold = 10 + rng.Next(0, 11) + depth * 4;
                TreasureItem = rng.Next(100) < 60 ? ItemFactory.CreateRandom(depth, rng, 10) : null;
                break;
            case RoomType.Trap:
                TrapDamage = 6 + depth * 2;
                break;
        }

        Generate();
    }

    // Une salle de départ, puis un boss toutes les 10 salles, sinon un type tiré au hasard.
    public static Room Create(int depth, Random rng)
    {
        RoomType type = depth % 10 == 0 ? RoomType.Boss : Roll(depth, rng);
        return new Room(depth, type, rng);
    }

    private static RoomType Roll(int depth, Random rng)
    {
        // Pas de repos ni de piège au tout début : le joueur est encore en pleine forme.
        int roll = rng.Next(depth <= 2 ? 75 : 100);
        if (roll < 45) return RoomType.Combat;
        if (roll < 60) return RoomType.Merchant;
        if (roll < 75) return RoomType.Treasure;
        if (roll < 90) return RoomType.Rest;
        return RoomType.Trap;
    }

    public string Title => Type switch
    {
        RoomType.Start => "Hall d'entrée",
        RoomType.Combat => "Salle de combat",
        RoomType.Merchant => "Échoppe",
        RoomType.Treasure => "Salle au trésor",
        RoomType.Rest => "Salle de repos",
        RoomType.Trap => "Salle piégée",
        RoomType.Boss => "Antre du boss",
        _ => "Salle"
    };

    // Indice affiché sur la porte qui mène à cette salle.
    public string Hint => Type switch
    {
        RoomType.Combat => "des bruits de combat",
        RoomType.Merchant => "une voix qui marchande",
        RoomType.Treasure => "une lueur dorée",
        RoomType.Rest => "la chaleur d'un feu de camp",
        RoomType.Trap => "un étrange cliquetis",
        RoomType.Boss => "un rugissement terrifiant",
        _ => "le silence"
    };

    private char CenterGlyph => Type switch
    {
        RoomType.Combat => 'E',
        RoomType.Boss => 'B',
        RoomType.Merchant => 'M',
        RoomType.Treasure => 'T',
        RoomType.Rest => 'F',
        RoomType.Trap => '^',
        _ => '.'
    };

    public void Generate()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                map[y, x] = (y == 0 || y == Height - 1 || x == 0 || x == Width - 1) ? '#' : '.';

        // 3 portes numérotées dans le mur du haut
        map[0, Width / 4] = '1';
        map[0, Width / 2] = '2';
        map[0, 3 * Width / 4] = '3';

        // l'entité de la salle au centre
        map[Height / 2, Width / 2] = CenterGlyph;

        // le joueur en bas, au milieu
        map[Height - 2, Width / 2] = 'P';
    }

    public void MarkCleared()
    {
        Cleared = true;
        map[Height / 2, Width / 2] = '.';
    }

    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine($"=== Salle {Depth} : {Title}{(Cleared ? " (terminée)" : "")} ===");
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                Console.Write(map[y, x]);
            Console.WriteLine();
        }
        Console.WriteLine("1-3 portes | P vous | E ennemi | B boss | M marchand | T trésor | F feu | ^ piège");
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        return map[y, x] != '#';
    }
}
