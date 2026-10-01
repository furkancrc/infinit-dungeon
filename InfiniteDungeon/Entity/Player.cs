using InfiniteDungeon.Items;

namespace InfiniteDungeon.Entity;

public class Player : Character
{
    private readonly List<Item> inventory = new();

    public int Money { get; private set; }
    public int Level { get; private set; } = 1;
    public int Xp { get; private set; }
    public int XpToNext => 12 + Level * 12;
    public Weapon? EquippedWeapon { get; private set; }
    public Armor? EquippedArmor { get; private set; }
    public IReadOnlyList<Item> Inventory => inventory;

    public int AttackPower => 5 + Level * 2 + (EquippedWeapon?.Damage ?? 0);
    public int Defense => EquippedArmor?.Defense ?? 0;

    public Player(string name) : base(name, 50)
    {
        Money = 20;
        var dagger = new Weapon("Dague rouillée", 6, 2);
        AddItem(dagger);
        Equip(dagger);
        AddItem(new HealPotion("Petite potion", 15, 25));
        AddItem(new HealPotion("Petite potion", 15, 25));
    }

    public override int TakeDamage(int amount) => base.TakeDamage(Math.Max(1, amount - Defense));

    public override AttackResult Attack(Character target, Random rng)
    {
        int damage = AttackPower + rng.Next(-1, 3);
        bool critical = rng.Next(100) < 15;
        if (critical)
            damage *= 2;

        int dealt = target.TakeDamage(Math.Max(1, damage));
        return new AttackResult(dealt, critical);
    }

    public void AddItem(Item item) => inventory.Add(item);

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
        if (item == EquippedWeapon) EquippedWeapon = null;
        if (item == EquippedArmor) EquippedArmor = null;
    }

    public void UseItem(Item item)
    {
        if (item.Use(this))
            RemoveItem(item);
    }

    public void Equip(Weapon weapon) => EquippedWeapon = weapon;

    public void Equip(Armor armor) => EquippedArmor = armor;

    public bool IsEquipped(Item item) => item == EquippedWeapon || item == EquippedArmor;

    public void AddMoney(int amount) => Money += Math.Max(0, amount);

    public bool SpendMoney(int amount)
    {
        if (amount > Money)
            return false;

        Money -= amount;
        return true;
    }

    // Retourne true si le joueur monte d'au moins un niveau.
    public bool GainXp(int amount)
    {
        Xp += amount;
        bool leveledUp = false;
        while (Xp >= XpToNext)
        {
            Xp -= XpToNext;
            Level++;
            MaxHealth += 10;
            Heal(MaxHealth / 4);
            leveledUp = true;
        }
        return leveledUp;
    }
}
