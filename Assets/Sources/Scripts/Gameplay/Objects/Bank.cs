using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Cards.Gameplay
{
    public class Bank : MonoBehaviour
    {
        [SerializeField] private List<Transform> _cells;

        [Inject] private LevelState _state;
        [Inject] private Level _level;
        [Inject] private Field _field;

        private List<Card> _cards = new ();

        private Card[] _cardsToRemove;
        private Sequence _mainSequence;

        private float _duration = 0.2f;
        private float _upScale = 1.25f;
        private float _downScale = 0.1f;

        private int _similarCount = 3;
        private int _minCleanCount = 3;

        private int _bankSize = 7;

        private Card _lastAddedCard;

        public int CardsCount => _cards.Count;

        public bool IsFull => _cards.Count == _bankSize;

        public bool CanUseCleaning => _cards.Count >= _minCleanCount;

        public bool CanUseCancel => _lastAddedCard != null;

        public void AddNewCard(Card card)
        {
            _cards.Add(card);
            _lastAddedCard = card;
        }

        public void Clear()
        {
            foreach (Card card in _cards)
                Destroy(card.gameObject);

            _cards.Clear();
            ClearLastMove();
        }

        public void PartialClean()
        {
            for (int i = 0; i < _minCleanCount; i++)
                _field.MoveToClearContainer(_cards[i]);

            _cards.RemoveRange(0, _minCleanCount);

            SetCardsInCells();
        }

        public void CancelMove()
        {
            _cards.Remove(_lastAddedCard);

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

        public async Task<Transform> GetEmptyCellTransform()
        {
            if (_mainSequence != null)
            {
                _mainSequence.Complete();

                await SequenceCallback();
            }

            foreach (var cell in _cells)
            {
                if (cell.childCount == 0)
                    return cell;
            }

            throw new ArgumentException("Не удалось получить пустую ячейку!");
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

            _cardsToRemove = matchGroup.ToArray();

            _mainSequence = DOTween.Sequence();

            foreach (Card card in _cardsToRemove)
            {
                Transform cardTransform = card.transform;

                _cards.Remove(card);

                _mainSequence.Insert(0, cardTransform.DOScale(_upScale, _duration).SetEase(Ease.OutBack));
                _mainSequence.Insert(_duration, cardTransform.DOScale(_downScale, _duration).SetEase(Ease.InQuad));
            }

            _mainSequence.OnComplete(async () =>
            {
                await SequenceCallback();

                _state.CardsCount.Value -= _similarCount;
            });
        }

        private void SetCardsInCells()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].transform.SetParent(_cells[i]);
                _cards[i].transform.localPosition = Vector3.zero;
            }

            ClearLastMove();
        }

        private async Task SequenceCallback()
        {
            foreach (Card card in _cardsToRemove)
                Destroy(card.gameObject);

            Array.Resize(ref _cardsToRemove, 0);

            await Task.Yield();

            if (_cards.Count > 0)
                SetCardsInCells();
        }

        private void ClearLastMove()
        {
            _lastAddedCard = null;
        }
    }
}