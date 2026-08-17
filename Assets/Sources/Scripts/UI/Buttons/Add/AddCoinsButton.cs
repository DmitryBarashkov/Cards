using UnityEngine;
using YG;

public class AddCoinsButton : ToggleButton
{
    [SerializeField] private WinGameScreen _winScreen;

    private string _rewardId = "MultiplyCoins";
    private int _coinsFactor = 2;

    public override void HandleClick()
    {
        Utils.ShowAdvForReward(_audioService, _rewardId, AddReward);
    }

    private void AddReward()
    {
        _winScreen.AddCoins(_coinsFactor);
        _audioService.Activate();
        SetEnabled(false);
    }
}
