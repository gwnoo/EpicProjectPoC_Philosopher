using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    private Dictionary<EquipmentSlot, EquipmentData> _equipment = new();
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private ClassType _classType;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor(); // 초기 색상 설정
    }

    public bool CanAccept(EquipmentData newEquip)
    {
        if (newEquip.classType != GetHeroClass()) return false;

        if (_equipment.TryGetValue(newEquip.slot, out var current))
            return newEquip.power > current.power;

        return true;
    }

    public void Equip(EquipmentData newEquip)
    {
        _equipment[newEquip.slot] = newEquip;
        UpdateColor();
    }

    public ClassType GetHeroClass()
    {
        return _classType;
    }

    private void UpdateColor()
    {
        Rarity highestRarity = Rarity.Common;

        foreach (var item in _equipment.Values)
        {
            if (item.rarity > highestRarity)
                highestRarity = item.rarity;
        }

        _spriteRenderer.color = EquipmentColor.GetColorByRarity(highestRarity);
    }

    public int GetPowerSum()
    {
        if (_equipment.Count == 0) return 5; // 기본값
        int total = 0;
        foreach (var eq in _equipment.Values)
            total += eq.power;

        return total;
    }


}
