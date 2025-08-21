using TMPro;
using UnityEngine;

public class MineralText : MonoBehaviour
{
    StorageBox storageBox;
    TextMeshProUGUI text;

    void Awake()
    {
        storageBox = FindAnyObjectByType<StorageBox>();
        text = GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        text.text = $"철: {storageBox.GetMaterialCount(EquipmentSlot.Weapon)}개\n" +
                      $"금: {storageBox.GetMaterialCount(EquipmentSlot.Body)}개\n" +
                      $"은: {storageBox.GetMaterialCount(EquipmentSlot.Boots)}개\n" +
                      $"청동: {storageBox.GetMaterialCount(EquipmentSlot.Gloves)}개\n" +
                      $"다이아몬드: {storageBox.GetMaterialCount(EquipmentSlot.Head)}개";
    }
}
