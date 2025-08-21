using UnityEngine;
using System.Collections.Generic;

public class StorageBox : MonoBehaviour
{
    private Dictionary<EquipmentSlot, int> _materials = new();

    private void Awake()
    {
        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
            _materials[slot] = 0;
    }

    public void AddMaterial(EquipmentSlot slot, int amount)
    {
        _materials[slot] += amount;
        Debug.Log($"[StorageBox] {slot} Àç·á +{amount} ¡æ ÃÑ: {_materials[slot]}");
    }

    public int GetMaterialCount(EquipmentSlot slot)
    {
        return _materials.TryGetValue(slot, out int value) ? value : 0;
    }
}
