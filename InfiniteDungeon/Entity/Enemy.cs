using InfiniteDungeon.Items;

namespace InfiniteDungeon.Entity;

public class Enemy : Character
{
    private readonly List<Item> inventory;

    public int Damage { get; }
    public int GoldReward { get; }
    public int XpReward { get; }
    public bool IsBoss { get; }

    public Enemy(string name, int health, int damage, int goldReward, int xpReward,
                 List<Item> loot, bool isBoss = false) : base(name, health)
    {
        Damage = damage;
        GoldReward = goldReward;
        XpReward = xpReward;
        inventory = loot;
        IsBoss = isBoss;
    }

    public override AttackResult Attack(Character target, Random rng)
    {
        int damage = rng.Next(Damage * 8 / 10, Damage * 12 / 10 + 1);
        int dealt = target.TakeDamage(Math.Max(1, damage));
        return new AttackResult(dealt, false);
    }

    public List<Item> DropLoot() => new(inventory);
}
