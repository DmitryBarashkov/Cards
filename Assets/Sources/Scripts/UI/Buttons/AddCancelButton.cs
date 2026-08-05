using UnityEngine;
using YG;
using Zenject;

public class AddCancelButton : EndScreenButton
{
    [SerializeField] private int _addCount = 3;
    
    [Inject] private PlayerStats _playerStats;

    private string _rewardId = "AddCancel";

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _audioService.Deactivate();

        YG2.RewardedAdvShow(_rewardId, () =>
        {

            YG2.saves.cancels += _addCount;
            _playerStats.currentCleanings.Value += _addCount;
            _audioService.Activate();
        });

        SetEnabled(false);
    }
}
