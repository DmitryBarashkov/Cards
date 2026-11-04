using Cards.Databases;
using Cards.Services;
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
        private UIService _uiService;
        private LoadLevelService _loadLevelService;
        private AdService _adService;
        private IAudioService _audioService;
        private int _levelCardsCount;
        private int _levelNumber;

        private bool _isActive;

        public int CardsCount => _levelCardsCount;

        public bool IsActive => _isActive;

        [Inject]
        public void Construct(
            LevelState state,
            LevelGenerator generator,
            LoadLevelService loadLevelService,
            Field field,
            Bank bank,
            UIService uiService,
            AdService adService,
            IAudioService audioService,
            int levelNumber)
        {
            _state = state;
            _field = field;
            _bank = bank;

            _generator = generator;
            _uiService = uiService;
            _loadLevelService = loadLevelService;
            _adService = adService;
            _audioService = audioService;

            _levelNumber = levelNumber;

            SetLevelState();
            StartLevel();
        }

        public void SetLevelState()
        {
            _isActive = true;
            _state.CardsCount.Value = _levelCardsCount = _field.CardsCount + _bank.CardsCount;
            _state.LevelNumber.Value = _levelNumber;
        }

        public void Restart()
        {
            var nodes = _generator.GetInitialNodes();

            _adService.ShowInterstitialAdv();
            _bank.Clear();
            _field.Initialize(nodes);

            SetLevelState();
        }

        public void ShowLoseScreen()
        {
            _uiService.ShowEndGameScreen(false);
        }

        public void ShowWinScreen()
        {
            _audioService.PlaySound(SoundType.Win);
            _isActive = false;
            _uiService.ShowEndGameScreen(true);
        }

        public void StartNextLevel()
        {
            _adService.ShowInterstitialAdv();

            _levelNumber++;

            StartLevel();
        }

        private void StartLevel()
        {
            LevelConfig config = _loadLevelService.GetLevelConfig();

            _generator.SetGeneratorParams(config);

            var levelCardList = config.LevelDifficulty == Difficulty.Hard
                ? _generator.GenerateHard()
                : _generator.GenerateEasyMedium();

            _field.Initialize(levelCardList);

            SetLevelState();
        }
    }
}