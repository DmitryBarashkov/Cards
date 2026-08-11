using YG;
using Zenject;
using System.Collections.Generic;

public enum ControlType
{
    Shuffles,
    Cleanings,
    Cancels
}

public class ShopService
{
    private PlayerStats _playerStats;
    private UIService _service;

    private Dictionary<ControlType, int> _prices = new Dictionary<ControlType, int>()
    {
        { ControlType.Shuffles, 75 },
        { ControlType.Cleanings, 100 },
        { ControlType.Cancels, 25 }
    };        

    [Inject]
    public void Construct(PlayerStats playerStats, UIService service)
    {
        _playerStats = playerStats;
        _service = service;
    }    
    
    public void ShowBuyScreen(ControlType type)
    {
        _service.ShowBuyScreen(type, _prices[type]);
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
        YG2.saves.shuffles += count;
        _playerStats.currentShuffles.Value += count;
    }

    private void PurchaseCleanings(int count)
    {
        UpdatePlayerCoins(ControlType.Cleanings);
        YG2.saves.cleanings += count;
        _playerStats.currentCleanings.Value += count;        
    }

    private void PurchaseCancels(int count)
    {
        UpdatePlayerCoins(ControlType.Cancels);
        YG2.saves.cancels += count;
        _playerStats.currentCancels.Value += count;
    }

    private void UpdatePlayerCoins(ControlType type)
    {
        int price = _prices[type];

        YG2.saves.coins -= price;
        _playerStats.currentCoins.Value -= price;
    }
}
