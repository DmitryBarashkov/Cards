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
    [Inject] private InputService _input;
    [Inject] private Field _field;

    private List<Card> _cards = new();   

    private float _duration = 0.2f;
    private float _upScale = 1.25f;
    private float _downScale = 0.1f;

    private int _similarCount = 3;
    private int _minCleanCount = 3;
    private int _emptyCellIndex = 0;

    private int _maxCellIndex = 6;
    private int _bankSize = 7;
    private int _bankMaxSize = 7;

    private Card _lastAddedCard;

    public int CardsCount => _cards.Count;

    public bool IsFull => _cards.Count == _bankSize;

    public Transform PlaceholderTransform => _cells[_emptyCellIndex];

    public void AddNewCard(Card card)
    {
        _cards.Add(card);
        _lastAddedCard = card;

        if (TryClearSimilarCards() == false)
        {
            _emptyCellIndex++;
            _emptyCellIndex = Mathf.Min(_maxCellIndex, _emptyCellIndex);
        }
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
        if (_cards.Count < _minCleanCount)
            return;

        for (int i = 0; i < _minCleanCount; i++)
            _field.MoveToClearContainer(_cards[i]);        

        _cards.RemoveRange(0, _minCleanCount);

        SetCardsInCells();
    }

    public void IncreaseBankSize()
    {
        if (_bankSize < _bankMaxSize)
            _bankSize++;
        else
            throw new ArgumentException("Нельзя увеличить банк выше максимума");
    }

    public void CancelMove()
    {
        if (_lastAddedCard == null)
            return;

        _cards.Remove(_lastAddedCard);

        if (_emptyCellIndex != _maxCellIndex)
            _emptyCellIndex--;

        if (_lastAddedCard.IsCleared)
            _field.MoveToClearContainer(_lastAddedCard);
        else        
            _lastAddedCard.MoveToField();
        
        ClearLastMove();
    }

    private bool TryClearSimilarCards()
    {
        if (_cards.Count >= _similarCount)
        {
            var matchGroup = _cards
                .GroupBy(card => card.Id)
                .FirstOrDefault(group => group.Count() >= _similarCount);

            if (matchGroup != null)
            {
                Sequence mainSequence = DOTween.Sequence();
                Card[] cardsToRemove = matchGroup.ToArray();

                foreach (Card card in cardsToRemove)
                {
                    Transform cardTransform = card.transform;

                    mainSequence.Insert(0, cardTransform.DOScale(_upScale, _duration).SetEase(Ease.OutBack));
                    mainSequence.Insert(_duration, cardTransform.DOScale(_downScale, _duration).SetEase(Ease.InQuad));
                }

                mainSequence.OnComplete(() =>
                {
                    foreach (Card card in cardsToRemove)
                    {
                        _cards.Remove(card);
                        Destroy(card.gameObject);
                        
                        _emptyCellIndex--;
                        _emptyCellIndex = Mathf.Max(0, _emptyCellIndex);
                    }                    

                    if (_cards.Count > 0)
                        SetCardsInCells();

                    _state.CardsCount.Value -= _similarCount;

                    _input.Activate();
                });

                return true;
            }
            else
            {
                _input.Activate();
                return false;
            }
        }
        else
        {
            _input.Activate();
            return false;
        }
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
}
