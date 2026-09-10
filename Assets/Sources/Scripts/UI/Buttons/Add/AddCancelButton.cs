using UnityEngine;
using YG;
using Zenject;

public class AddCancelButton : ToggleButton
{
    [SerializeField] private int _addCount = 3;

    [Inject] private PlayerStats _playerStats;

    private string _rewardId = "AddCancel";

    public override void HandleClick()
    {
        Utils.ShowAdvForReward(_audioService, _rewardId, AddReward);
    }

    private void AddReward()
    {
        YG2.saves.Cancels += _addCount;
        _playerStats.CurrentCancels.Value += _addCount;
        SetEnabled(false);
    }
}
