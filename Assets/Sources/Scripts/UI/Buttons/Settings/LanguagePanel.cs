using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class LanguagePanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    [Inject] private AudioService _audioService;

    private RectTransform _rectTransform;

    private bool _isOpen = false;

    private float _expandedWidth = 315f;
    private float _collapsedWidth = 0f;

    private float _duration = 0.3f;
    private Ease _easeType = Ease.InOutQuad;

    public event UnityAction LanguageChanged;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(_collapsedWidth, _rectTransform.sizeDelta.y);

        _canvasGroup.alpha = 0f;
        SetInteraction(false);
    }

    public void ToggleMenu()
    {
        _audioService.PlaySound(SoundType.ExpandPanel);
        _rectTransform.DOKill();
        _canvasGroup.DOKill();

        if (!_isOpen)
        {
            _rectTransform.DOSizeDelta(new Vector2(_expandedWidth, _rectTransform.sizeDelta.y), _duration).SetEase(_easeType);
            _canvasGroup.DOFade(1f, _duration);

            SetInteraction(true);
            _isOpen = true;
        }
        else
        {
            _rectTransform.DOSizeDelta(new Vector2(_collapsedWidth, _rectTransform.sizeDelta.y), _duration).SetEase(_easeType);
            _canvasGroup.DOFade(0f, _duration).OnComplete(() => _rectTransform.gameObject.SetActive(true));

            SetInteraction(false);
            _isOpen = false;
        }
    }

    public void CreateSetLanguageButtons(SetLanguageButton prefab, string currentLanguage, Dictionary<string, Sprite> languages)
    {
        SetLanguageButton firstButton = Instantiate(prefab, _canvasGroup.transform);

        firstButton.Initialize(currentLanguage, languages[currentLanguage], this, _audioService);

        foreach (var language in languages)
        {
            if (language.Key == currentLanguage)
                continue;

            SetLanguageButton setLanguageButton = Instantiate(prefab, _canvasGroup.transform);

            setLanguageButton.Initialize(language.Key, language.Value, this, _audioService);
        }
    }

    public void ChangeLanguage(string language)
    {
        LanguageChanged?.Invoke();

        SortSetLanguageButtons(language);
    }

    private void SortSetLanguageButtons(string language)
    {
        SetLanguageButton[] buttons = GetComponentsInChildren<SetLanguageButton>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetLanguageKey() == language)
            {
                buttons[i].transform.SetAsFirstSibling();
                break;
            }
        }
    }

    private void SetInteraction(bool state)
    {
        _canvasGroup.interactable = state;
        _canvasGroup.blocksRaycasts = state;
    }
}
