using UniRx;
using Zenject;

public class PlayerStats
{
    public ReactiveProperty<int> CurrentCoins = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> CurrentShuffles = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> CurrentCleanings = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> CurrentCancels = new ReactiveProperty<int>(0);

    [Inject]
    public void Construct(int coins, int shuffles, int cleanings, int cancels)
    {
        CurrentCoins.Value = coins;
        CurrentShuffles.Value = shuffles;
        CurrentCleanings.Value = cleanings;
        CurrentCancels.Value = cancels;
    }
}
