using UnityEngine;

public static class HeroBalance
{
    public static float GetAttackCooldown(ClassType type)
    {
        return type switch
        {
            ClassType.Archer => 0.6f,
            ClassType.Mage => 1.5f,
            ClassType.Mechanic => 2.0f,
            ClassType.Alchemist => 1.2f,
            ClassType.Sniper => 3.0f,
            _ => 1.5f
        };
    }

    public static float GetDamageMultiplier(ClassType type)
    {
        return type switch
        {
            ClassType.Archer => 0.5f,
            ClassType.Mage => 1.0f,
            ClassType.Mechanic => 1.6f,
            ClassType.Alchemist => 0.7f,
            ClassType.Sniper => 2.5f,
            _ => 1.0f
        };
    }

    public static Color GetEndColor(ClassType type)
    {
        return type switch
        {
            ClassType.Archer => Color.black,
            ClassType.Mage => Color.red,
            ClassType.Mechanic => Color.blue,
            ClassType.Alchemist => Color.green,
            ClassType.Sniper => Color.cyan,
            _ => Color.white
        };
    }
}
