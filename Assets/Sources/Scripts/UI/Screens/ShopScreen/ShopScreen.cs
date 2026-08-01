using Zenject;
using UniRx;

public class ShopScreen : UIScreen
{
    [Inject]
    private PlayerStats _playerStats;

    public override void Setup()
    {
        _playerStats.currentCoins.Skip(1).Subscribe((newCoins) =>
        {
            UpdateItems();
        })
        .AddTo(this);

        UpdateItems();
        _gameObject.SetActive(true);
    }

    public void Close() => _gameObject.SetActive(false);

    private void UpdateItems()
    {
        
    }
}
