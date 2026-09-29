using Cards.UI;
using UnityEngine;
using YG;
using Zenject;

public class BuyByAdsButton : UIButton
{
    [SerializeField] private UIScreen _screen;
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
                _rewardId = string.Empty;
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

        _screen.Close();
    }

    private void AddReward()
    {
        switch (_rewardId)
        {
            case "AddShuffles":
                YG2.saves.Shuffles++;
                _playerStats.CurrentShuffles.Value++;
                break;
            case "AddCleanings":
                YG2.saves.Cleanings++;
                _playerStats.CurrentCleanings.Value++;
                break;
            case "AddCancels":
                YG2.saves.Cancels += _cancelCount;
                _playerStats.CurrentCancels.Value = YG2.saves.Cancels;
                break;
            case "AddCoins":
                YG2.saves.Coins += _coinsCount;
                _playerStats.CurrentCoins.Value = YG2.saves.Coins;
                break;
            default:
                break;
        }
    }
}
