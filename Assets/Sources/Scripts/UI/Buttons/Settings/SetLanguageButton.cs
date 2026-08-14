using UnityEngine;
using UnityEngine.UI;
using YG;

public class SetLanguageButton : UIButton
{
    [SerializeField] private Image _image;

    private string _language;
    private LanguagePanel _panel;

    public void Initialize(string language, Sprite sprite, LanguagePanel panel, AudioService audioService)
    {
        _audioService = audioService;
        _language = language;
        _image.sprite = sprite;
        _panel = panel;
    }

    public override void HandleClick()
    {
        YG2.SwitchLanguage(_language);

        _panel.ChangeLanguage(_language);
        _panel.ToggleMenu();
    }

    public string GetLanguageKey()
    {
        return _language;
    }
}
