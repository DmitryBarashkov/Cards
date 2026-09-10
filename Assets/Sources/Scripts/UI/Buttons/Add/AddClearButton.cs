using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Zenject;

public class AddClearButton : ToggleButton
{
    [Inject] private PlayerStats _playerStats;

    private string _rewardId = "AddClear";

    public override void HandleClick()
    {
        Utils.ShowAdvForReward(_audioService, _rewardId, AddReward);
    }

    private void AddReward()
    {
        YG2.saves.Cleanings++;
        _playerStats.CurrentCleanings.Value++;
        SetEnabled(false);
    }
}
