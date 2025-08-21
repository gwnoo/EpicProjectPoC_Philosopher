using UnityEngine;

public static class EquipmentColor
{
    public static Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return Color.gray;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.6f, 0f, 1f); // 보라색
            case Rarity.Legendary: return new Color(1f, 0.5f, 0f); // 주황색
            default: return Color.white;
        }
    }
}
