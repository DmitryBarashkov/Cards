using Zenject;

public class UnlockCellButton : UIButton
{
    private const string _rewardId = "UnlockBankCell";

    [Inject] private Bank _bank;
    
    public override void HandleClick()
    {
        Utils.ShowAdvForReward(_audioService, _rewardId, () => _bank.IncreaseBankSize());
    }
}
