using Cards.Gameplay;
using Cards.UI;
using UnityEngine;
using YG;
using Zenject;

public class NextLevelButton : UIButton
{
    [SerializeField] private UIScreen _screen;

    [Inject] private Level _level;

    public override void HandleClick()
    {
        _level.StartNextLevel();
        _screen.Close();
    }
}
