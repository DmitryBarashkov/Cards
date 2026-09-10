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

        if (_stats.CurrentCancels.Value > 0)
        {
            YG2.saves.Cancels--;
            _stats.CurrentCancels.Value--;
            _bank.CancelMove();
        }
        else
        {
            _shopService.ShowBuyScreen(ControlType.Cancels);
        }
    }
}
