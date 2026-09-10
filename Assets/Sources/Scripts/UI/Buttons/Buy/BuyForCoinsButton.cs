using Zenject;

public class BuyForCoinsButton : ToggleButton
{
    [Inject] private ShopService _service;

    private ControlType _type;

    public override void HandleClick()
    {
        int count = _type == ControlType.Cancels ? 3 : 1;

        _service.PurchaseControl(_type, count);
    }

    public void SetProductType(ControlType type)
    {
        _type = type;
    }
}
