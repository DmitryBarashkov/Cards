using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class BuyByMoneyButton : UIButton
{
    [SerializeField] private ShopItem _item;
    [SerializeField] private TextMeshProUGUI _priceText;

    [Inject] private ShopService _service;

    protected override void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);
        YG2.onPurchaseSuccess += OnSuccess;

        if (YG2.purchases.Length > 0)
            UpdatePriceUI();
    }

    protected override void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
        YG2.onPurchaseSuccess -= OnSuccess;
    }

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);

        YG2.BuyPayments(_item.Id);
    }

    private void OnSuccess(string id)
    {
        _service.BuyInApp(id);
    }

    private void UpdatePriceUI()
    {
        foreach (var purchase in YG2.purchases)
        {
            if (purchase.id == _item.Id)
            {
                _priceText.text = purchase.price;
                return;
            }
        }

        _priceText.text = "1 Ян";
    }
}
