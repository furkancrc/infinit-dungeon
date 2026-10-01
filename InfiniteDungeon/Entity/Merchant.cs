using InfiniteDungeon.Items;

namespace InfiniteDungeon.Entity;

public class Merchant
{
    private readonly List<Item> inventory;

    public string Name { get; }
    public IReadOnlyList<Item> Inventory => inventory;

    public Merchant(string name, List<Item> stock)
    {
        Name = name;
        inventory = stock;
    }

    public int BuyPrice(Item item) => Math.Max(1, item.Price / 2);

    // Le marchand vend un objet au joueur.
    public bool Sell(Item item, Player player)
    {
        if (!inventory.Contains(item) || !player.SpendMoney(item.Price))
            return false;

        inventory.Remove(item);
        player.AddItem(item);
        return true;
    }

    // Le marchand rachète un objet du joueur (à moitié prix).
    public bool Buy(Item item, Player player)
    {
        if (!player.Inventory.Contains(item))
            return false;

        player.RemoveItem(item);
        player.AddMoney(BuyPrice(item));
        inventory.Add(item);
        return true;
    }

    public void ShowInventory()
    {
        if (inventory.Count == 0)
        {
            Console.WriteLine("  (rien à vendre)");
            return;
        }

        for (int i = 0; i < inventory.Count; i++)
            Console.WriteLine($"  {i + 1}. {inventory[i]} - {inventory[i].Price} or");
    }
}
