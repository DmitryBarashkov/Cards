using Zenject;
using UnityEngine;

public class NextLevelButton : UIButton
{
    [SerializeField] private UIScreen _screen;
    
    [Inject] private Level _level;
    
    public override void HandleClick()
    {
        _level.Restart();
        _screen.Close();
    }
}
