using TMPro;
using UnityEngine;

public class PlantText : MonoBehaviour
{
    SeedBox seedBox;
    TextMeshProUGUI text;

    void Awake()
    {
        seedBox = FindAnyObjectByType<SeedBox>();
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (seedBox == null) return;
        text.text = $"¾¾¾Ñ: {seedBox.SeedCount}°³\n" +
                      $"º¸¸®: {seedBox.BarleyCount}°³";
    }

}
