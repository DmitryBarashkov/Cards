using Zenject;

public class AddCellButton : ToggleButton
{
    [Inject] private Bank _bank;
    
    public override void HandleClick()
    {
        Utils.UnlockBankCell(_audioService, _bank);

        SetEnabled(false);
    }
}
