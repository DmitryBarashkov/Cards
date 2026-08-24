using Zenject;

public class OpenShopButton : UIButton
{
    [Inject] private ShopService _shopService;
    
    public override void HandleClick()
    {
        _shopService.ShowShopScreen();
    }
}
