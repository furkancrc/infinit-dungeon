using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items;

public enum Rarity
{
    Commun,
    Rare,
    Epique,
    Legendaire
}

public abstract class Item
{
    public string Name { get; }
    public string Kind { get; }
    public int Price { get; }
    public Rarity Rarity { get; }

    protected Item(string name, string kind, int price, Rarity rarity = Rarity.Commun)
    {
        Name = name;
        Kind = kind;
        Price = price;
        Rarity = rarity;
    }

    public abstract string Description { get; }

    // Retourne true si l'objet est consommé par son utilisation.
    public abstract bool Use(Player player);

    public override string ToString() => $"{Name} [{Rarity}] - {Description}";
}
