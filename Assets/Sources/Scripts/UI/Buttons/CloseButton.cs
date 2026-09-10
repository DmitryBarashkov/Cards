using UnityEngine;
using Zenject;

public class CloseButton : UIButton
{
    [SerializeField] private GameObject _screen;

    [Inject] private AdService _service;

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _screen.SetActive(false);
        _service.SetActive(true);
    }
}
