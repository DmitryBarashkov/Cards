using UnityEngine;
using YG;
using Zenject;

public class NextLevelButton : UIButton
{
    [SerializeField] private UIScreen _screen;

    [Inject] private Level _level;

    public override void HandleClick()
    {
        YG2.InterstitialAdvShow();

        _level.Restart();
        _screen.Close();
    }
}
