using System.Collections.Generic;
using Cards.Databases;
using YG;
using Zenject;

public enum ControlType
{
    Shuffles,
    Cleanings,
    Cancels,
}

public class ShopService
{
    private PlayerStats _playerStats;
    private UIService _service;
    private InAppsDatabase _database;

    private Dictionary<ControlType, int> _prices = new Dictionary<ControlType, int>()
    {
        { ControlType.Shuffles, 75 },
        { ControlType.Cleanings, 100 },
        { ControlType.Cancels, 25 },
    };

    [Inject]
    public void Construct(PlayerStats playerStats, UIService service, InAppsDatabase database)
    {
        _playerStats = playerStats;
        _service = service;
        _database = database;
    }

    public void BuyInApp(string id)
    {
        if (_database.TryGetItem(id, out var product))
        {
            if (product.IsOnceBuy)
            {
                switch (product.Id)
                {
                    case "1":
                        YG2.saves.IsBeginnerSetBought = true;
                        YG2.saves.IsAdsDisabled = true;
                        break;
                    case "3":
                        YG2.saves.IsAdsDisabled = true;
                        break;
                    default:
                        break;
                }

                YG2.SaveProgress();
            }
        }
    }

    public void ShowBuyScreen(ControlType type)
    {
        _service.ShowBuyScreen(type, _prices[type]);
    }

    public void ShowShopScreen()
    {
        _service.ShowShop();
    }

    public void PurchaseControl(ControlType type, int count)
    {
        switch (type)
        {
            case ControlType.Shuffles:
                PurchaseShuffles(count);
                break;
            case ControlType.Cleanings:
                PurchaseCleanings(count);
                break;
            case ControlType.Cancels:
                PurchaseCancels(count);
                break;
            default:
                break;
        }
    }

    private void PurchaseShuffles(int count)
    {
        UpdatePlayerCoins(ControlType.Shuffles);
        YG2.saves.Shuffles += count;
        _playerStats.CurrentShuffles.Value += count;
    }

    private void PurchaseCleanings(int count)
    {
        UpdatePlayerCoins(ControlType.Cleanings);
        YG2.saves.Cleanings += count;
        _playerStats.CurrentCleanings.Value += count;
    }

    private void PurchaseCancels(int count)
    {
        UpdatePlayerCoins(ControlType.Cancels);
        YG2.saves.Cancels += count;
        _playerStats.CurrentCancels.Value += count;
    }

    private void UpdatePlayerCoins(ControlType type)
    {
        int price = _prices[type];

        YG2.saves.Coins -= price;
        _playerStats.CurrentCoins.Value -= price;
    }
}
