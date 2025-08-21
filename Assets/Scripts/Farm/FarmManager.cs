using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    private List<CropTile> _tiles = new List<CropTile>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            CropTile tile = child.GetComponent<CropTile>();
            if (tile != null)
                _tiles.Add(tile);
        }
    }

    private void Start()
    {
        FindAnyObjectByType<TurnManager>().OnTurnStart += OnNewTurn;
    }

    public List<CropTile> GetAllTiles() => _tiles;

    public List<CropTile> GetReadyToHarvest()
    {
        return _tiles.FindAll(tile => tile.CanHarvest());
    }

    public List<CropTile> GetEmptyTiles()
    {
        return _tiles.FindAll(tile => tile.State == CropState.Empty);
    }

    public void OnNewTurn()
    {
        foreach (var tile in _tiles)
            tile.GrowTurn();
    }
}
