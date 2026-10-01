using InfiniteDungeon.Items;

namespace InfiniteDungeon.Factories;

public static class ItemFactory
{
    private static readonly string[] WeaponNames =
        { "Épée courte", "Hache de guerre", "Lame d'acier", "Lame runique", "Épée des anciens" };
    private static readonly int[] WeaponDamage = { 4, 7, 11, 16, 23 };

    private static readonly string[] ArmorNames =
        { "Veste en cuir", "Cotte de mailles", "Plastron d'acier", "Armure runique", "Armure draconique" };
    private static readonly int[] ArmorDefense = { 1, 2, 4, 6, 9 };

    private static readonly string[] PotionNames =
        { "Petite potion", "Potion de soin", "Grande potion", "Élixir" };
    private static readonly int[] PotionHeal = { 25, 45, 75, 120 };

    private static int Tier(int depth, int length, Random rng) =>
        Math.Min((depth + rng.Next(0, 3)) / 4, length - 1);

    public static Rarity RollRarity(int depth, Random rng, int bonus = 0)
    {
        int roll = rng.Next(100) + Math.Min(depth, 30) + bonus;
        if (roll < 70) return Rarity.Commun;
        if (roll < 92) return Rarity.Rare;
        if (roll < 110) return Rarity.Epique;
        return Rarity.Legendaire;
    }

    private static double Multiplier(Rarity rarity) => rarity switch
    {
        Rarity.Rare => 1.3,
        Rarity.Epique => 1.7,
        Rarity.Legendaire => 2.2,
        _ => 1.0
    };

    public static Weapon CreateWeapon(int depth, Random rng, int bonus = 0)
    {
        int tier = Tier(depth, WeaponNames.Length, rng);
        Rarity rarity = RollRarity(depth, rng, bonus);
        int damage = (int)Math.Round((WeaponDamage[tier] + rng.Next(0, 3)) * Multiplier(rarity));
        return new Weapon(WeaponNames[tier], damage * 7, damage, rarity);
    }

    public static Armor CreateArmor(int depth, Random rng, int bonus = 0)
    {
        int tier = Tier(depth, ArmorNames.Length, rng);
        Rarity rarity = RollRarity(depth, rng, bonus);
        int defense = Math.Max(1, (int)Math.Round(ArmorDefense[tier] * Multiplier(rarity)));
        return new Armor(ArmorNames[tier], defense * 25, defense, rarity);
    }

    public static HealPotion CreatePotion(int depth, Random rng)
    {
        int tier = Tier(depth, PotionNames.Length, rng);
        int heal = PotionHeal[tier];
        return new HealPotion(PotionNames[tier], heal * 3 / 5, heal);
    }

    public static Item CreateRandom(int depth, Random rng, int bonus = 0)
    {
        int roll = rng.Next(100);
        if (roll < 30) return CreateWeapon(depth, rng, bonus);
        if (roll < 55) return CreateArmor(depth, rng, bonus);
        return CreatePotion(depth, rng);
    }

    public static List<Item> CreateShopStock(int depth, Random rng)
    {
        var stock = new List<Item>();

        int potions = 2 + rng.Next(0, 2);
        for (int i = 0; i < potions; i++)
            stock.Add(CreatePotion(depth, rng));

        int weapons = 1 + rng.Next(0, 2);
        for (int i = 0; i < weapons; i++)
            stock.Add(CreateWeapon(depth, rng));

        int armors = 1 + rng.Next(0, 2);
        for (int i = 0; i < armors; i++)
            stock.Add(CreateArmor(depth, rng));

        return stock;
    }
}
