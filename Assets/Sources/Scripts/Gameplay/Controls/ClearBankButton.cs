using Cards.Gameplay;
using YG;
using Zenject;

public class ClearBankButton : UIButton
{
    [Inject] private Bank _bank;
    [Inject] private PlayerStats _stats;
    [Inject] private ShopService _shopService;

    public override void HandleClick()
    {
        if (_bank.CanUseCleaning == false)
            return;

        if (_stats.CurrentCleanings.Value > 0)
        {
            YG2.saves.Cleanings--;
            _stats.CurrentCleanings.Value--;
            _bank.PartialClean();
        }
        else
        {
            _shopService.ShowBuyScreen(ControlType.Cleanings);
        }
    }
}
