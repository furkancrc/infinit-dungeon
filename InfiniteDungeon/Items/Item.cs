public class Item
{
    private string name;
    private string type;
    private int price;
    private int damage;

    public Item(string name, string type, int price, int damage)
    {
        this.name = name;
        this.type = type;
        this.price = price;
        this.damage = damage;
    }

    public void UsePlayer(Player player)
    {
        switch (Type)
        {
            case "potion":
                player.Health += Damage;
                Console.WriteLine($"+{Damage} PV");
                break;

            default:
                Console.WriteLine("Cet objet ne peut pas être utilisé.");
                break;
        }
    }