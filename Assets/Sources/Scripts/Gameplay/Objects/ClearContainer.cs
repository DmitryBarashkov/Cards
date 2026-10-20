using System.Collections.Generic;
using Cards.Gameplay;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ClearContainer : MonoBehaviour
{
    [Inject] private CardFactory _factory;

    [SerializeField] private List<CardCell> _cells;

    private RectTransform _rectTransform;
    private List<Card> _cards = new ();

    private float _clearDuration = 0.4f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _factory.GetHeight() * 1.5f);
    }

    public void AddNewCard(Card card)
    {
        CardCell clearCell = GetClearCell();
        Transform cellTransform = clearCell.transform;

        clearCell.SetEmpty(false);

        _cards.Add(card);
        card.RectTransform.SetParent(cellTransform);

        card.RectTransform.DOMove(cellTransform.position, _clearDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                card.SetCleared();
            });
    }

    public void DeleteCard(Card card)
    {
        CardCell cell = card.transform.GetComponentInParent<CardCell>();

        if (cell.transform.childCount == 1)
            cell.SetEmpty(true);

        _cards.Remove(card);
    }

    public void Clear()
    {
        foreach (CardCell cell in _cells)
            cell.SetEmpty(true);

        _cards.Clear();
    }

    private CardCell GetClearCell()
    {
        int minCellItemsCount = 0;
        int minCellCountIndex = 0;

        for (int i = 0; i < _cells.Count; i++)
        {
            CardCell cell = _cells[i];

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
