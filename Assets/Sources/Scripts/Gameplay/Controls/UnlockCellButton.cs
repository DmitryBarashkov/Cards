using YG;
using Zenject;

public class UnlockCellButton : UIButton
{
    [Inject] private Bank _bank;
    
    private string _rewardId = "UnlockBankCell";
    
    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _audioService.Deactivate();

        YG2.RewardedAdvShow(_rewardId, () =>
        {
            _bank.IncreaseBankSize();
            _audioService.Activate();
        });

        _button.gameObject.SetActive(false);
    }
}
