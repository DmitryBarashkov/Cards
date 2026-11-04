using UnityEngine;

namespace Cards.UI
{
    public class LevelGeneratorButton : UIButton
    {
        [SerializeField] private LevelGeneratorScreen _screen;

        public override void HandleClick()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _screen.GenerateLevel();
            _screen.Close();
        }
    }
}