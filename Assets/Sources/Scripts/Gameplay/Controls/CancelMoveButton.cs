using YG;
using Zenject;

public class CancelMoveButton : UIButton
{
    [Inject] private Bank _bank;
    [Inject] private PlayerStats _stats;
    [Inject] private ShopService _shopService;

    public override void HandleClick()
    {
        if (_bank.CanUseCancel == false)
            return;

        if (_stats.currentCancels.Value > 0)
        {
            YG2.saves.cancels--;
            _stats.currentCancels.Value--;
            _bank.CancelMove();
        }
        else
            _shopService.ShowBuyScreen(ControlType.Cancels);        
    }
}
