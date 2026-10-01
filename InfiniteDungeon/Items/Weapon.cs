using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items;

public sealed class Weapon : Item
{
    public int Damage { get; }

    public Weapon(string name, int price, int damage, Rarity rarity = Rarity.Commun)
        : base(name, "Arme", price, rarity)
    {
        Damage = damage;
    }

    public override string Description => $"+{Damage} dégâts";

    public override bool Use(Player player)
    {
        player.Equip(this);
        Console.WriteLine($"Vous équipez {Name}.");
        return false;
    }
}
