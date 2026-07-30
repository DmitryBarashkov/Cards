using Zenject;

public class ReloadLevelButton : UIButton
{
    [Inject] private Level _level;
    
    public override void HandleClick()
    {
        _level.Restart();
    }
}
