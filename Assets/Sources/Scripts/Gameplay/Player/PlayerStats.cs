using UniRx;
using Zenject;

public class PlayerStats
{
    public ReactiveProperty<int> currentCoins = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> currentShuffles = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> currentCleanings = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> currentCancels = new ReactiveProperty<int>(0);

    [Inject]
    public void Construct(int coins, int shuffles, int cleanings, int cancels)
    {
        currentCoins.Value = coins;
        currentShuffles.Value = shuffles;
        currentCleanings.Value = cleanings;
        currentCancels.Value = cancels;
    }
}
