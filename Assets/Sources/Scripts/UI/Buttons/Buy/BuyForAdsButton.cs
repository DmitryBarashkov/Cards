using YG;
using Zenject;

public class BuyForAdsButton : ToggleButton
{
    [Inject] private PlayerStats _playerStats;

    private string _rewardId;
    private int _cancelCount = 3;

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
    
    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _audioService.Deactivate();

        YG2.RewardedAdvShow(_rewardId, () =>
        {

            AddReward();
            _audioService.Activate();
        });

        SetEnabled(false);
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
                _playerStats.currentCancels.Value += _cancelCount;
                break;
            default:
                break;
        }
    }
}
