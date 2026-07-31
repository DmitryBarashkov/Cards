using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ClearContainer : MonoBehaviour
{
    [SerializeField] private List<ClearCell> _cells;

    private List<Card> _cards = new();
    
    private float _clearDuration = 0.4f;    
    
    public void AddNewCard(Card card)
    {
        ClearCell clearCell = GetClearCell();
        Transform cellTransform = clearCell.transform;

        clearCell.SetEmpty(false);
        
        _cards.Add(card);
        card.RectTransform.SetParent(cellTransform);

        card.RectTransform.DOMove(cellTransform.position, _clearDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {                               
                card.SetCleared();
            });
    }

    public void DeleteCard(Card card)
    {
        ClearCell cell = card.transform.GetComponentInParent<ClearCell>();
        
        if (cell.transform.childCount == 1)
            cell.SetEmpty(true);

        _cards.Remove(card);
    }

    public void Clear()
    {
        foreach (ClearCell cell in _cells)
            cell.SetEmpty(true);
        
        _cards.Clear();
    }

    private ClearCell GetClearCell()
    {
        int minCellItemsCount = 0;
        int minCellCountIndex = 0;
        
        for (int i = 0; i < _cells.Count; i++)
        {
            ClearCell cell = _cells[i];
            
            if (cell.IsEmpty)
                return cell;

            int childCount = cell.transform.childCount;

            if (minCellItemsCount == 0)
            {
                minCellItemsCount = childCount;
                minCellCountIndex = i;
            }
            else
            {
                if (childCount < minCellItemsCount)
                {
                    minCellItemsCount = childCount;
                    minCellCountIndex = i;
                }
            }
        }

        return _cells[minCellCountIndex];
    }
}
