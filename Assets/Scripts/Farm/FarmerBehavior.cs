using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerBehavior : MonoBehaviour
{
    private SeedBox _seedBox;
    private FarmManager _farmManager;
    private FarmerMover _mover;

    private bool _busy = false;

    private void Awake()
    {
        _seedBox = FindAnyObjectByType<SeedBox>();
        _farmManager = FindAnyObjectByType<FarmManager>();
        _mover = GetComponent<FarmerMover>();
    }

    private void Start()
    {
        FindAnyObjectByType<TurnManager>().OnTurnStart += StartRoutine;
    }

    void StartRoutine()
    {
        if (!_busy)
            StartCoroutine(FarmCycle());
    }

    IEnumerator FarmCycle()
    {
        _busy = true;

        // 1. ¼öÈ® ¿ì¼±
        foreach (var tile in _farmManager.GetReadyToHarvest())
        {
            _mover.MoveTo(tile.transform.position);
            yield return new WaitUntil(() => _mover.ReachedDestination());
            yield return new WaitForSeconds(0.5f);

            tile.Harvest();

            // ¼öÈ® ÈÄ º¸¸® + ¾¾¾Ñ »óÀÚ¿¡ ³Ö±â
            _mover.MoveTo(_seedBox.transform.position);
            yield return new WaitUntil(() => _mover.ReachedDestination());
            yield return new WaitForSeconds(0.5f);

            _seedBox.AddSeed(2);   // ¾¾¾Ñ 2°³ È¹µæ
            _seedBox.AddBarley(); // º¸¸® 1°³ È¹µæ
        }


        // 2. ½É±â
        foreach (var tile in _farmManager.GetEmptyTiles())
        {
            if (_seedBox.SeedCount <= 0)
                break;

            // »óÀÚ ÀÌµ¿
            _mover.MoveTo(_seedBox.transform.position);
            yield return new WaitUntil(() => _mover.ReachedDestination());
            yield return new WaitForSeconds(0.5f);

            if (!_seedBox.UseSeed()) break;

            // Å¸ÀÏ ÀÌµ¿
            _mover.MoveTo(tile.transform.position);
            yield return new WaitUntil(() => _mover.ReachedDestination());
            yield return new WaitForSeconds(0.5f);

            tile.Plant();
        }

        _busy = false;
    }
}
