using Cards.Gameplay;
using Cards.UI;
using UnityEngine;
using Zenject;

public class NextLevelButton : UIButton
{
    [SerializeField] private UIScreen _screen;

    [Inject] private Level _level;

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _level.StartNextLevel();
        _screen.Close();
    }
}
