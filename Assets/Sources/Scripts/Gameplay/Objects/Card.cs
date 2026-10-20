using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using static Cards.Databases.CardsDatabase;

namespace Cards.Gameplay
{
    public class Card : UIButton
    {
        private readonly Color _darkenedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        [SerializeField] private Image _image;
        [SerializeField] private Image _background;

        private Field _field;
        private Bank _bank;
        private Transform _gameplayContainer;

        private int _id;
        private float _duration = 0.2f;
        private bool _inBank = false;
        private bool _isCleared = false;
        private Color _normalColor;

        private Vector3 _initialPosition;

        public RectTransform RectTransform => _rectTransform;

        public int Id => _id;

        public bool IsCleared => _isCleared;

        [Inject]
        public void Construct(InputService input, Field field, Bank bank, GameplayContainer gameplayContainer)
        {
            _field = field;
            _bank = bank;
            _gameplayContainer = gameplayContainer.transform;
            _normalColor = _background.color;
        }

        public override void HandleClick()
        {
            if (_field.IsCardOverlapped(this))
                _rectTransform.DOShakePosition(_duration);
            else
                ExecuteCardAction();
        }

        public void InitializeCardData(CardType cardType)
        {
            _id = cardType.Id;
            _image.sprite = cardType.Sprite;

            _image.transform.localScale = Vector3.one;
        }

        public void SetCleared()
        {
            _inBank = false;
            _isCleared = true;
        }

        public void MoveToField()
        {
            if (_initialPosition == Vector3.zero)
                throw new ArgumentException("У карточки нет исходной позиции");

            _rectTransform.SetParent(_gameplayContainer.transform);
            _rectTransform.DOMove(_initialPosition, _duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _rectTransform.SetParent(_field.transform);
                _rectTransform.SetAsLastSibling();

                _field.AddCard(this);
                _inBank = false;
            });
        }

        public void ToggleHighlight(bool isNeedHighlight)
        {
            _background.color = isNeedHighlight ? _normalColor : _darkenedColor;
        }

        private void ExecuteCardAction()
        {
            if (_inBank == false)
            {
                _field.DeleteCard(this);
                MoveToBank();
            }
        }

        private async void MoveToBank()
        {
            if (_bank == null || _bank.IsFull)
                return;

            _initialPosition = transform.position;

            Transform emptyCellTransform = await _bank.GetEmptyCellTransform();

            _rectTransform.SetParent(_gameplayContainer.transform);
            _rectTransform.DOMove(emptyCellTransform.position, _duration).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _bank.CheckSimilarCards();
                });

            _rectTransform.SetParent(emptyCellTransform);

            _bank.AddNewCard(this);
            _inBank = true;
        }
    }
}