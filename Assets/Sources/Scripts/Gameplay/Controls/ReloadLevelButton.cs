using Zenject;
using UnityEngine;

public class ReloadLevelButton : UIButton
{
    [SerializeField] private GameObject _screen;
    
    [Inject] private Level _level;
    
    public override void HandleClick()
    {
        _level.Restart();

        if (_screen != null)
            _screen.SetActive(false);
    }
}
