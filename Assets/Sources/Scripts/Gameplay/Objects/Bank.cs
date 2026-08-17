using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Bank : MonoBehaviour
{
    [SerializeField] private List<Transform> _cells;
    
    [Inject] private LevelState _state;
    [Inject] private Level _level;
    [Inject] private Field _field;

    private List<Card> _cards = new();   

    private float _duration = 0.2f;
    private float _upScale = 1.25f;
    private float _downScale = 0.1f;

    private int _similarCount = 3;
    private int _minCleanCount = 3;
    private int _emptyCellIndex = 0;

    private int _maxCellIndex = 6;
    private int _bankSize = 5;
    private int _bankMaxSize = 7;

    private Card _lastAddedCard;

    public int CardsCount => _cards.Count;
    public bool IsFull => _cards.Count == _bankSize;
    public bool CanUseCleaning => _cards.Count >= _minCleanCount;
    public bool CanUseCancel => _lastAddedCard != null;
    public bool IsAllCellsEnabled => _bankSize == _bankMaxSize;
    public Transform PlaceholderTransform => _cells[_emptyCellIndex];

    public void AddNewCard(Card card)
    {
        _cards.Add(card);
        _lastAddedCard = card;

        _emptyCellIndex++;        
    }

    public void Clear()
    {
        foreach (Card card in _cards)
            Destroy(card.gameObject);
        
        _cards.Clear();
        _emptyCellIndex = 0;
        ClearLastMove();
    }

    public void PartialClean()
    {
        for (int i = 0; i < _minCleanCount; i++)
            _field.MoveToClearContainer(_cards[i]);        

        _cards.RemoveRange(0, _minCleanCount);

        SetCardsInCells();
    }

    public void IncreaseBankSize()
    {
        if (_bankSize < _bankMaxSize)
        {
            _bankSize++;
            UpdateCellsIcons();
        }
        else
            throw new ArgumentException("Нельзя увеличить банк выше максимума");
    }

    public void CancelMove()
    {
        _cards.Remove(_lastAddedCard);

        if (_emptyCellIndex != _maxCellIndex)
            _emptyCellIndex--;

        if (_lastAddedCard.IsCleared)
            _field.MoveToClearContainer(_lastAddedCard);
        else        
            _lastAddedCard.MoveToField();
        
        ClearLastMove();
    }

    public void CheckSimilarCards()
    {
        TryClearSimilarCards();

        if (this.IsFull)
            _level.ShowLoseScreen();        
        else if (_field.CardsCount == 0 && _cards.Count == 0)
            _level.ShowWinScreen();
    }

    private void TryClearSimilarCards()
    {
        if (_cards.Count < _similarCount)
            return;
        
        var matchGroup = _cards
            .GroupBy(card => card.Id)
            .FirstOrDefault(group => group.Count() >= _similarCount);

        if (matchGroup == null)
            return;

        Sequence mainSequence = DOTween.Sequence();
        Card[] cardsToRemove = matchGroup.ToArray();

        foreach (Card card in cardsToRemove)
        {
            Transform cardTransform = card.transform;

            _cards.Remove(card);

            mainSequence.Insert(0, cardTransform.DOScale(_upScale, _duration).SetEase(Ease.OutBack));
            mainSequence.Insert(_duration, cardTransform.DOScale(_downScale, _duration).SetEase(Ease.InQuad));
        }

        mainSequence.OnComplete(() =>
        {
            foreach (Card card in cardsToRemove)
            {
                Destroy(card.gameObject);

                _emptyCellIndex--;                
            }

            if (_cards.Count > 0)
                SetCardsInCells();

            _state.CardsCount.Value -= _similarCount;
        });
    }

    private void SetCardsInCells()
    {
        _emptyCellIndex = 0;
        
        foreach (Card activeCard in _cards)
        {
            activeCard.transform.SetParent(_cells[_emptyCellIndex]);
            activeCard.transform.localPosition = Vector3.zero;
            _emptyCellIndex++;
        }

        ClearLastMove();
    }

    private void ClearLastMove()
    {
        _lastAddedCard = null;        
    }

    private void UpdateCellsIcons()
    {
        foreach (var cell in _cells)
        {
            UnlockCellButton button = cell.GetComponentInChildren<UnlockCellButton>();

            if (button != null)
            {
                button.gameObject.SetActive(false);
                break;
            }
        }
    }
}
