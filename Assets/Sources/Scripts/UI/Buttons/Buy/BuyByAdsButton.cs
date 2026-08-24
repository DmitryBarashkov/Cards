using YG;
using Zenject;

public class BuyByAdsButton : UIButton
{
    [Inject] private PlayerStats _playerStats;

    private string _rewardId;
    private int _cancelCount = 3;
    private int _coinsCount = 150;

    public void SetReward(ControlType type)
    {
        switch (type)
        {
            case ControlType.Shuffles:
                _rewardId = "AddShuffles";
                break;
            case ControlType.Cleanings:
                _rewardId = "AddCleanings";
                break;
            case ControlType.Cancels:
                _rewardId = "AddCancels";
                break;
            default:
                _rewardId = "";
                break;
        }
    }

    public void SetReward(int coins)
    {
        _rewardId = "AddCoins";
        _coinsCount = coins;
    }

    public override void HandleClick()
    {
        Utils.ShowAdvForReward(_audioService, _rewardId, AddReward);
    }

    private void AddReward()
    {
        switch (_rewardId)
        {
            case "AddShuffles":
                YG2.saves.shuffles++;
                _playerStats.currentShuffles.Value++;
                break;
            case "AddCleanings":
                YG2.saves.cleanings++;
                _playerStats.currentCleanings.Value++;
                break;
            case "AddCancels":
                YG2.saves.cancels += _cancelCount;
                _playerStats.currentCancels.Value = YG2.saves.cancels;
                break;
            case "AddCoins":
                YG2.saves.coins += _coinsCount;
                _playerStats.currentCoins.Value = YG2.saves.coins;
                break;
            default:
                break;
        }
    }
}
