using UnityEngine;

namespace Cards.UI
{
    public class LevelGeneratorButton : UIButton
    {
        [SerializeField] private LevelGeneratorScreen _screen;

        public override void HandleClick()
        {
            _screen.GenerateLevel();
            _screen.Close();
        }
    }
}