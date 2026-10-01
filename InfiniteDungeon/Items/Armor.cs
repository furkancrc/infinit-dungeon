using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items;

public sealed class Armor : Item
{
    public int Defense { get; }

    public Armor(string name, int price, int defense, Rarity rarity = Rarity.Commun)
        : base(name, "Armure", price, rarity)
    {
        Defense = defense;
    }

    public override string Description => $"+{Defense} défense";

    public override bool Use(Player player)
    {
        player.Equip(this);
        Console.WriteLine($"Vous équipez {Name}.");
        return false;
    }
}
