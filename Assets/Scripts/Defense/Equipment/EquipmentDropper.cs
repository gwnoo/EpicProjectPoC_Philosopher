using UnityEngine;

public class EquipmentDropper : MonoBehaviour
{
    [SerializeField] private GameObject equipmentPrefab;
    private float dropChance = 0.5f;

    public void DropEquipment()
    {
        if (Random.value > dropChance)
        {
            Debug.Log("장비 드롭 실패");
            return;
        }

        EquipmentData generatedData = GenerateRandomEquipment();
        GameObject newEquipment = Instantiate(equipmentPrefab, transform.position, Quaternion.identity);

        Equipment eq = newEquipment.GetComponent<Equipment>();
        eq.Initialize(generatedData);

        Debug.Log($"장비 드롭됨: {generatedData.itemName} / {generatedData.classType} / {generatedData.slot} / Power {generatedData.power}");
    }

    private EquipmentData GenerateRandomEquipment()
    {
        EquipmentSlot slot = (EquipmentSlot)Random.Range(0, System.Enum.GetValues(typeof(EquipmentSlot)).Length);
        ClassType classType = (ClassType)Random.Range(0, System.Enum.GetValues(typeof(ClassType)).Length);
        Rarity rarity = RollRarity();

        int basePower = 5;
        int rarityMultiplier = 1 + (int)rarity;
        int power = basePower * rarityMultiplier + Random.Range(0, 5);

        string name = $"{rarity} {classType} {slot}";

        return new EquipmentData(name, slot, classType, power, rarity);
    }

    private Rarity RollRarity()
    {
        float roll = Random.value;
        if (roll < 0.6f) return Rarity.Common;
        if (roll < 0.85f) return Rarity.Rare;
        if (roll < 0.97f) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
