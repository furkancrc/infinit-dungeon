using InfiniteDungeon.Entity;
using InfiniteDungeon.Items;

namespace InfiniteDungeon.Factories;

public static class EnemyFactory
{
    private record Template(string Name, int Health, int Damage);

    private static readonly Template[] Templates =
    {
        new("Gobelin", 20, 4),
        new("Squelette", 24, 5),
        new("Loup", 18, 6),
        new("Orc", 34, 7),
        new("Spectre", 28, 8),
        new("Troll", 50, 9)
    };

    private static readonly string[] BossNames =
        { "Dragon ancien", "Seigneur Liche", "Golem ancestral", "Démon des abysses" };

    // Les ennemis les plus forts n'apparaissent que plus profondément dans le donjon.
    public static Enemy Create(int depth, Random rng)
    {
        int pool = Math.Min(Templates.Length, 2 + depth / 2);
        Template template = Templates[rng.Next(pool)];

        // La difficulté augmente de plus en plus vite avec la profondeur.
        int health = template.Health + depth * 4 + depth * depth / 6;
        int damage = template.Damage + depth * 3 / 5 + depth * depth / 35;
        int gold = 4 + rng.Next(0, 6) + depth * 2;
        int xp = 10 + depth * 3;

        var loot = new List<Item>();
        if (rng.Next(100) < 35)
            loot.Add(ItemFactory.CreateRandom(depth, rng));

        return new Enemy(template.Name, health, damage, gold, xp, loot);
    }

    public static Enemy CreateBoss(int depth, Random rng)
    {
        string name = BossNames[(depth / 10 - 1) % BossNames.Length];

        int health = 30 + depth * 4 + depth * depth / 2;
        int damage = 5 + depth * 3 / 5 + depth * depth / 35;
        int gold = 50 + depth * 5;
        int xp = 40 + depth * 5;

        var loot = new List<Item>
        {
            ItemFactory.CreateWeapon(depth, rng, 25),
            ItemFactory.CreateArmor(depth, rng, 25),
            ItemFactory.CreatePotion(depth, rng)
        };

        return new Enemy(name, health, damage, gold, xp, loot, isBoss: true);
    }
}
