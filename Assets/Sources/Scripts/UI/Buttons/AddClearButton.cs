using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Zenject;

public class AddClearButton : EndScreenButton
{
    [Inject] private PlayerStats _playerStats;

    private string _rewardId = "AddClear";

    public override void HandleClick()
    {
        _audioService.PlaySound(SoundType.ButtonClick);
        _audioService.Deactivate();

        YG2.RewardedAdvShow(_rewardId, () =>
        {
            
            YG2.saves.cleanings++;
            _playerStats.currentCleanings.Value++;
            _audioService.Activate();
        });

        SetEnabled(false);
    }
}
