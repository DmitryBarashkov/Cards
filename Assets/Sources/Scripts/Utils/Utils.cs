using YG;

public static class Utils
{
    public static void UnlockBankCell(IAudioService audioService, Bank bank)
    {
        audioService.PlaySound(SoundType.ButtonClick);
        audioService.Deactivate();

        YG2.RewardedAdvShow("UnlockBankCell", () =>
        {
            bank.IncreaseBankSize();
            audioService.Activate();
        });
    }
}
