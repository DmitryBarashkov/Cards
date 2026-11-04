using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using static Cards.Databases.CardsDatabase;

namespace Cards.Gameplay
{
    public class Card : UIButton
    {
        private readonly Color _darkenedColor = new Color32(161, 161, 161, 255);

        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        [SerializeField] private ParticleSystem _moveEffect;
        [SerializeField] private float _moveDuration = 0.4f;
        [SerializeField] private float _installDuration = 0.3f;

        [Header("Настройки анимации потряхивания")]
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private Vector3 _shakeStrength = new Vector3(5, 5, 0);
        [SerializeField] private int _vibrato = 25;
        [SerializeField] private float _randomness = 90;
        [SerializeField] private bool _snapping = false;
        [SerializeField] private bool _fadeOut = true;
        [SerializeField] private ShakeRandomnessMode _shakeRandomnessMode = ShakeRandomnessMode.Harmonic;

        private Field _field;
        private Bank _bank;
        private Transform _gameplayContainer;

        private int _id;
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

            _moveEffect.Stop();
        }

        public override void HandleClick()
        {
            if (_field.IsCardOverlapped(this))
            {
                _rectTransform.DOShakeAnchorPos(_shakeDuration, _shakeStrength, _vibrato, _randomness, _snapping, _fadeOut, _shakeRandomnessMode);
                _audioService.PlaySound(SoundType.CardBlocked);
            }
            else
            {
                ExecuteCardAction();
            }
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
            _rectTransform.DOMove(_initialPosition, _moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
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
                _moveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _moveEffect.Play();
                _audioService.PlaySound(SoundType.CardClick);

                _field.DeleteCard(this);

                MoveToBank();
            }
        }

        private async void MoveToBank()
        {
            if (_bank == null || _bank.IsFull)
                return;

            Vector3 lastPosition = transform.position;
            Transform emptyCellTransform = await _bank.GetEmptyCellTransform();

            _initialPosition = transform.position;
            _rectTransform.SetParent(_gameplayContainer.transform);

            _audioService.PlaySound(SoundType.CardMove);
            _rectTransform.DOMove(emptyCellTransform.position, _moveDuration)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() =>
                {
                    Vector3 moveDirection = transform.position - lastPosition;

                    if (moveDirection.sqrMagnitude > 0.0001f)
                    {
                        Vector3 oppositeDirection = -moveDirection.normalized;

                        _moveEffect.transform.rotation = Quaternion.LookRotation(Vector3.forward, oppositeDirection);
                    }

                    lastPosition = transform.position;
                })
                .OnComplete(() =>
                {
                    _moveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _rectTransform.SetParent(emptyCellTransform);

                    _audioService.PlaySound(SoundType.CardSetBank);
                    PerformBankInstallAnimation();

                    _bank.CheckSimilarCards();
                });

            _bank.AddNewCard(this);
            _inBank = true;
        }

        private void PerformBankInstallAnimation()
        {
            _rectTransform.DOComplete();

            _rectTransform.localRotation = Quaternion.Euler(0, 0, 5f);

            _rectTransform.DOScale(new Vector3(0.8f, 0.8f, 1f), _installDuration);
            _rectTransform.DORotate(Vector3.zero, _installDuration).SetEase(Ease.OutBack);
            _rectTransform.DOScale(Vector3.one, _installDuration).SetEase(Ease.OutBack);
        }
    }
}