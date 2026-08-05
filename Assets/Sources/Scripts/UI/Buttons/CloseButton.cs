using UnityEngine;

public class CloseButton : UIButton
{
    [SerializeField] private GameObject _screen;

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _screen.SetActive(false);
    }
}
