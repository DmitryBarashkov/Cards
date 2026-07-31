using UnityEngine;
using Zenject;
using DG.Tweening;
using static CardsDatabase;
using UnityEngine.UI;
using System;

public class Card : UIButton
{
    [SerializeField] Image _image;
        
    private InputService _input;
    private Field _field;
    private Bank _bank;
    private GameplayCanvas _canvas;
        
    private int _id;
    private float _duration = 0.4f;
    private bool _inBank = false;   
    private bool _isCleared = false;

    private Vector3 _initialPosition;

    public RectTransform RectTransform => _rectTransform;
    public int Id => _id;
    public bool InBank => _inBank;
    public bool IsCleared => _isCleared;

    [Inject]
    public void Construct(InputService input, Field field, Bank bank, GameplayCanvas canvas)
    {
        _input = input;
        _field = field;
        _bank = bank;
        _canvas = canvas;
    }
    
    public override void HandleClick()
    {
        if (_field.IsCardOverlapped(this))
            Debug.Log($"{gameObject.name} заблокирована: сверху есть другая карточка!");
        else
            ExecuteCardAction();
    }

    public void InitializeCardData(CardType cardType)
    {
        _id = cardType.id;
        _image.color = cardType.color;
        _image.sprite = cardType.sprite;

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

        _rectTransform.SetParent(_canvas.transform);
        
        _rectTransform.DOMove(_initialPosition, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                _rectTransform.SetParent(_field.transform);
                _rectTransform.SetAsLastSibling();
                _field.AddCard(this);
                _inBank = false;
            });
    }

    private void ExecuteCardAction()
    {
        if (_input.IsActive && _inBank == false)
        {
            _field.DeleteCard(this);
            MoveToBank();
        }
    }

    private void MoveToBank()
    {
        if (_bank == null || _bank.IsFull) 
            return;

        _initialPosition = transform.position;
        
        _input.Deactivate();

        _rectTransform.SetParent(_canvas.transform);

        _rectTransform.DOMove(_bank.PlaceholderTransform.position, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                _bank.AddNewCard(this);
                _rectTransform.SetParent(_bank.PlaceholderTransform);                
                _inBank = true;                
            });
    }
}
