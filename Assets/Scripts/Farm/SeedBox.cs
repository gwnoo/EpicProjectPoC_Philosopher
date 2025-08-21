using UnityEngine;

public class SeedBox : MonoBehaviour
{
    public int SeedCount { get; private set; } = 10;
    public int BarleyCount { get; private set; } = 0;

    /// <summary>¾¾¾Ñ Ãß°¡</summary>
    public void AddSeed(int amount = 1)
    {
        SeedCount += amount;
        Debug.Log($"[SeedBox] ¾¾¾Ñ +{amount} ¡æ ÃÑ: {SeedCount}");
    }

    /// <summary>¾¾¾Ñ »ç¿ë (¼º°ø ½Ã true ¹ÝÈ¯)</summary>
    public bool UseSeed(int amount = 1)
    {
        if (SeedCount >= amount)
        {
            SeedCount -= amount;
            Debug.Log($"[SeedBox] ¾¾¾Ñ -{amount} ¡æ ³²Àº: {SeedCount}");
            return true;
        }
        return false;
    }

    /// <summary>º¸¸® ¼öÈ®¹° Ãß°¡</summary>
    public void AddBarley(int amount = 1)
    {
        BarleyCount += amount;
        Debug.Log($"[SeedBox] º¸¸® +{amount} ¡æ ÃÑ: {BarleyCount}");
    }

    /// <summary>º¸¸® ¼öÈ®¹° »ç¿ë (ÇÊ¿äÇÏ´Ù¸é)</summary>
    public bool UseBarley(int amount = 1)
    {
        if (BarleyCount >= amount)
        {
            BarleyCount -= amount;
            Debug.Log($"[SeedBox] º¸¸® -{amount} ¡æ ³²Àº: {BarleyCount}");
            return true;
        }
        return false;
    }
}
