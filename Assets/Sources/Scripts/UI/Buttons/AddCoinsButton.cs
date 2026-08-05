using UnityEngine;
using YG;

public class AddCoinsButton : EndScreenButton
{
    [SerializeField] private WinGameScreen _winScreen;

    private string _rewardId = "MultiplyCoins";
    private int _coinsFactor = 2;

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _audioService.Deactivate();

        YG2.RewardedAdvShow(_rewardId, () =>
        {
            _winScreen.AddCoins(_coinsFactor);
            _audioService.Activate();
        });

        SetEnabled(false);
    }
}
