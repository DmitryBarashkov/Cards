using YG;
using Zenject;

public class ShopService
{
    [Inject] private PlayerStats _playerStats;

    public void PurchaseShuffles(int count)
    {
        YG2.saves.shuffles += count;
        YG2.SaveProgress();

        _playerStats.currentShuffles.Value += count;
    }

    public void PurchaseCleanings(int count)
    {
        YG2.saves.cleanings += count;
        YG2.SaveProgress();

        _playerStats.currentCleanings.Value += count;
    }

    public void PurchaseCancels(int count)
    {
        YG2.saves.cancels += count;
        YG2.SaveProgress();

        _playerStats.currentCancels.Value += count;
    }
}
