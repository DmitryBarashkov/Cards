using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using static CardsDatabase;

public class Card : UIButton
{
    [SerializeField] private Image _image;

    private Field _field;
    private Bank _bank;
    private Transform _gameplayContainer;

    private int _id;
    private float _duration = 0.2f;
    private bool _inBank = false;
    private bool _isCleared = false;

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
        _rectTransform.DOMove(_initialPosition, _duration).SetEase(Ease.OutQuad);
        _rectTransform.SetParent(_field.transform);
        _rectTransform.SetAsLastSibling();

        _field.AddCard(this);
        _inBank = false;
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
