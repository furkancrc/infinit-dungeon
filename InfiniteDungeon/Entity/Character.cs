namespace InfiniteDungeon.Entity;

public readonly record struct AttackResult(int Damage, bool Critical);

public abstract class Character
{
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public bool IsAlive => Health > 0;

    protected Character(string name, int maxHealth)
    {
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

    // Retourne les dégâts réellement subis.
    public virtual int TakeDamage(int amount)
    {
        int damage = Math.Max(0, amount);
        Health = Math.Max(0, Health - damage);
        return damage;
    }

    // Retourne les PV réellement rendus.
    public int Heal(int amount)
    {
        int healed = Math.Min(Math.Max(0, amount), MaxHealth - Health);
        Health += healed;
        return healed;
    }

    public abstract AttackResult Attack(Character target, Random rng);
}
