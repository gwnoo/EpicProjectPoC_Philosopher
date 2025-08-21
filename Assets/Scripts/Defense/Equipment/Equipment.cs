using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentData data;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(EquipmentData generatedData)
    {
        data = generatedData;
        name = data.itemName;

        // 색상 적용
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.color = EquipmentColor.GetColorByRarity(data.rarity);
    }
}




public enum Rarity { Common, Rare, Epic, Legendary }
public enum EquipmentSlot { Weapon, Head, Body, Gloves, Boots }
public enum ClassType { Archer, Mage, Mechanic, Alchemist, Sniper }
