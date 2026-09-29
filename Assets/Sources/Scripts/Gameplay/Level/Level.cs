using YG;
using Zenject;

namespace Cards.Gameplay
{
    public class Level
    {
        private LevelState _state;
        private Field _field;
        private Bank _bank;
        private LevelGenerator _generator;
        private UIService _service;

        private int _levelCardsCount;
        private int _levelNumber;

        private bool _isActive;

        public int CardsCount => _levelCardsCount;

        public bool IsActive => _isActive;

        [Inject]
        public void Construct(LevelState state, LevelGenerator generator, Field field, Bank bank, UIService service, int levelNumber)
        {
            _state = state;
            _field = field;
            _bank = bank;
            _generator = generator;
            _service = service;
            _levelNumber = levelNumber;

            Initialize();
        }

        public void SetLevelState()
        {
            _isActive = true;
            _state.CardsCount.Value = _levelCardsCount = _field.CardsCount + _bank.CardsCount;
            _state.LevelNumber.Value = _levelNumber;
        }

        public void Restart()
        {
            YG2.InterstitialAdvShow();

            var nodes = _generator.GetInitialNodes();

            _bank.Clear();
            _field.Initialize(nodes);
            SetLevelState();
        }

        public void ShowLoseScreen()
        {
            _service.ShowEndGameScreen(false);
        }

        public void ShowWinScreen()
        {
            _isActive = false;
            _service.ShowEndGameScreen(true);
        }

        private void Initialize()
        {
            SetLevelState();
        }
    }
}