using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

public class LanguageButton : UIButton
{
    [SerializeField] private Image _image;
    [SerializeField] private LanguagePanel _panel;
    [SerializeField] private SetLanguageButton _setLanguageButton;

    [Inject(Id = "Languages")] private Dictionary<string, Sprite> _languages;

    protected override void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);
        _panel.LanguageChanged += SetLanguage;

        SetLanguage();
        InitializeLanguageButtons();
    }

    protected override void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
        _panel.LanguageChanged -= SetLanguage;
    }

    public override void HandleClick()
    {
        _panel.ToggleMenu();
    }

    private void SetLanguage()
    {
        _image.sprite = _languages[YG2.lang];

        YG2.GetLeaderboard("Score");
    }

    private void InitializeLanguageButtons()
    {
        _panel.CreateSetLanguageButtons(_setLanguageButton, YG2.lang, _languages);
    }
}
