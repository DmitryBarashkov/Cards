using UniRx;
using Zenject;

public class ShopScreen : UIScreen
{
    [Inject] private PlayerStats _playerStats;

    public override void Setup()
    {
        _playerStats.CurrentCoins.Skip(1).Subscribe((newCoins) =>
        {
            UpdateItems();
        })
        .AddTo(this);

        UpdateItems();

        base.Setup();
    }

    private void UpdateItems()
    {
    }
}
