using YG;
using UnityEngine;
using System;

public static class Utils
{
    public static Color GetColorFromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;
                
        return Color.white;
    }

    public static void ShowAdvForReward(IAudioService audioService, string rewardId, Action callback)
    {
        audioService.PlaySound(SoundType.ButtonClick);
        audioService.Deactivate();

        YG2.RewardedAdvShow(rewardId, () =>
        {
            audioService.Activate();
            callback();
        });
    }
}
