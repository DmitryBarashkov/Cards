using YG;
using Zenject;

public class ShuffleCardsButton : UIButton
{
    [Inject] private Field _field;
    [Inject] private PlayerStats _stats;
    [Inject] private ShopService _shopService;
    
    public override void HandleClick()
    {
        if (_stats.currentShuffles.Value > 0)
        {
            YG2.saves.shuffles--;
            
            _stats.currentShuffles.Value--;
            _field.ShuffleCards();
        }
        else
            _shopService.ShowBuyScreen(ControlType.Shuffles);
    }
}
