using UnityEngine;

public class CardNode
{
    public Vector3Int GridPosition;
    public int CardTypeId;         
    public bool IsOccupied;

    public Vector3 GetWorldPosition(float cardWidth, float cardHeight)
    {
        float visualShiftX = 0.05f;
        float visualShiftY = 0.05f;

        return new Vector3(
            GridPosition.x * (cardWidth * 0.5f) + (GridPosition.z * visualShiftX),
            GridPosition.y * (cardHeight * 0.5f) + (GridPosition.z * visualShiftY),
            0
        );
    }
}
