using UnityEngine;

[System.Serializable]
public class EquipmentData
{
    public string itemName;
    public EquipmentSlot slot;
    public ClassType classType;
    public int power;
    public Rarity rarity;

    public EquipmentData(string name, EquipmentSlot slot, ClassType classType, int power, Rarity rarity)
    {
        this.itemName = name;
        this.slot = slot;
        this.classType = classType;
        this.power = power;
        this.rarity = rarity;
    }
}
