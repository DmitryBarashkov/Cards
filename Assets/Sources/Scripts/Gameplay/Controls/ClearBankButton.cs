using Zenject;

public class ClearBankButton : UIButton
{
    [Inject] private Bank _bank;
    
    public override void HandleClick()
    {
        _bank.PartialClean();
    }
}
