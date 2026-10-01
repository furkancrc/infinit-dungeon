using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items;

public sealed class HealPotion : Item
{
    public int Heal { get; }

    public HealPotion(string name, int price, int heal, Rarity rarity = Rarity.Commun)
        : base(name, "Potion", price, rarity)
    {
        Heal = heal;
    }

    public override string Description => $"rend {Heal} PV";

    public override bool Use(Player player)
    {
        if (player.Health >= player.MaxHealth)
        {
            Console.WriteLine("Vos PV sont déjà au maximum.");
            return false;
        }

        int healed = player.Heal(Heal);
        Console.WriteLine($"Vous récupérez {healed} PV.");
        return true;
    }
}
