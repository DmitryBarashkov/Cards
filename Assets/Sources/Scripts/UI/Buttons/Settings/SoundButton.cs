using UnityEngine;
using UnityEngine.UI;

public class SoundButton : UIButton
{
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;
    [SerializeField] private Image _image;

    private bool _isOn;

    protected override void OnEnable()
    {
        base.OnEnable();

        _isOn = _audioService.GetSoundOn();
        SetSprite();
    }

    public override void HandleClick()
    {
        if (_isOn)
            _audioService.PlaySound(SoundType.ButtonClick);

        Toggle();

        _audioService.SetSound(_isOn);

        SetSprite();
    }

    private void Toggle()
    {
        _isOn = !_isOn;
    }

    private void SetSprite()
    {
        _image.sprite = _isOn ? _onSprite : _offSprite;
    }
}
