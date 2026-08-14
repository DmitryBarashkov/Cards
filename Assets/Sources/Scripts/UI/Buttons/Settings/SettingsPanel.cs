using DG.Tweening;
using UnityEngine;
using Zenject;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _gearIcon;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Inject] private AudioService _audioService;

    private RectTransform _rectTransform;

    private bool _isOpen = false;

    private float _expandedHeight = 380f;
    private float _collapsedHeight = 125f;

    private float _duration = 0.3f;
    private Ease _easeType = Ease.InOutQuad;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _isOpen = false;

        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _collapsedHeight);
        _canvasGroup.alpha = 0f;
        SetInteraction(false);
    }

    public void ToggleMenu()
    {
        _audioService.PlaySound(SoundType.ExpandPanel);
        _rectTransform.DOKill();
        _canvasGroup.DOKill();

        if (_gearIcon != null)
            _gearIcon.DOKill();

        if (!_isOpen)
        {
            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _expandedHeight), _duration).SetEase(_easeType);
            _canvasGroup.DOFade(1f, _duration);

            if (_gearIcon != null)
                _gearIcon.DORotate(new Vector3(0, 0, -180f), _duration, RotateMode.FastBeyond360);

            SetInteraction(true);
            _isOpen = true;
        }
        else
        {
            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _collapsedHeight), _duration).SetEase(_easeType);
            _canvasGroup.DOFade(0f, _duration);

            if (_gearIcon != null)
                _gearIcon.DORotate(Vector3.zero, _duration);

            SetInteraction(false);
            _isOpen = false;
        }
    }

    private void SetInteraction(bool state)
    {
        _canvasGroup.interactable = state;
        _canvasGroup.blocksRaycasts = state;
    }
}
