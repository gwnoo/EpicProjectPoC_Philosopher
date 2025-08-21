using UnityEngine;

public enum CropState { Empty, Growing, Ready }

[RequireComponent(typeof(SpriteRenderer))]
public class CropTile : MonoBehaviour
{
    public CropState State { get; private set; } = CropState.Empty;
    private int _turnsLeftToGrow;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void Plant()
    {
        if (State == CropState.Empty)
        {
            State = CropState.Growing;
            _turnsLeftToGrow = 3;
            UpdateColor();
        }
    }

    public void GrowTurn()
    {
        if (State == CropState.Growing)
        {
            _turnsLeftToGrow--;
            if (_turnsLeftToGrow <= 0)
            {
                State = CropState.Ready;
                UpdateColor();
            }
        }
    }

    public bool CanHarvest() => State == CropState.Ready;

    public void Harvest()
    {
        if (State == CropState.Ready)
        {
            State = CropState.Empty;
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        switch (State)
        {
            case CropState.Empty:
                _spriteRenderer.color = Color.gray;
                break;
            case CropState.Growing:
                _spriteRenderer.color = Color.green;
                break;
            case CropState.Ready:
                _spriteRenderer.color = Color.yellow;
                break;
        }
    }
}
