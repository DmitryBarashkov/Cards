using Zenject;

public class CancelMoveButton : UIButton
{
    [Inject] private Bank _bank;
    
    public override void HandleClick()
    {
        _bank.CancelMove();
    }
}
