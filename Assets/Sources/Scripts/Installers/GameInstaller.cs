using Cards.Gameplay;
using Cards.Services;
using Cards.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace Cards.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Префабы объектов")]
        [SerializeField] private Bank _bankPrefab;
        [SerializeField] private Field _fieldPrefab;
        [SerializeField] private Card _cardPrefab;

        [Header("Префабы экранов")]
        [SerializeField] private UIScreen _winGameScreen;
        [SerializeField] private UIScreen _loseGameScreen;
        [SerializeField] private UIScreen _shopScreen;
        [SerializeField] private UIScreen _buyScreen;
        [SerializeField] private UIScreen _generateLevelScreen;

        [Header("Контейнеры для экранов")]
        [SerializeField] private Transform _endGameContainer;
        [SerializeField] private Transform _shopContainer;
        [SerializeField] private Transform _generateLevelContainer;
        [SerializeField] private GameplayContainer _gameplayContainer;

        [Header("Сервисы")]
        [SerializeField] private CanvasScaler[] _canvasScales;
        [SerializeField] private GameObject _advCountContainer;
        [SerializeField] private TextMeshProUGUI _advCountText;

        private int _coins;
        private int _shuffles;
        private int _cleanings;
        private int _cancels;
        private int _levelNumber;

        public override void InstallBindings()
        {
            LoadPlayerData();
            BindPlayer();

            BindScreens();
            BindContainers();

            BindServices();
            BindLevel();
            BindGameObjects();
        }

        private void BindContainers()
        {
            Container.BindInstance(_endGameContainer).WithId("EndContainer");
            Container.BindInstance(_shopContainer).WithId("ShopContainer");
            Container.BindInstance(_generateLevelContainer).WithId("GenerateContainer");
        }

        private void BindScreens()
        {
            Container.BindInstance(_winGameScreen).WithId("Win");
            Container.BindInstance(_loseGameScreen).WithId("Lose");
            Container.BindInstance(_generateLevelScreen).WithId("Generate");

            if (_shopScreen != null)
                Container.BindInstance(_shopScreen).WithId("Shop");

            if (_buyScreen != null)
                Container.BindInstance(_buyScreen).WithId("Buy");
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<ShopService>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<UIService>()
                .AsSingle()
                .NonLazy();

            Container.BindInstance(_advCountContainer).WithId("AdWarning");
            Container.BindInstance(_advCountText).WithId("AdvCountText");

            Container.BindInterfacesAndSelfTo<AdService>()
            .AsSingle()
            .WithArguments(_advCountText)
            .NonLazy();

            Container.BindInterfacesAndSelfTo<SizeAdapter>().AsSingle().WithArguments(_canvasScales).NonLazy();

            Container.BindFactory<Transform, GameObject, UIScreen, UIScreen.Factory>()
                .FromMethod((container, parent, prefab) =>
                {
                    GameObject screen = container.InstantiatePrefab(prefab, parent);

                    return screen.GetComponent<UIScreen>();
                });
        }

        private void BindLevel()
        {
            Container.Bind<CardFactory>().AsSingle().WithArguments(_cardPrefab);
            Container.Bind<CardNode>().AsTransient();
            Container.Bind<LevelGenerator>().AsSingle().NonLazy();

            Container.Bind<LevelState>().AsSingle().NonLazy();
            Container.Bind<Level>()
                .AsSingle()
                .WithArguments(_levelNumber)
                .NonLazy();
        }

        private void BindGameObjects()
        {
            Container.Bind<GameplayContainer>().FromComponentInHierarchy().AsSingle();

            Container.Bind<Bank>()
                .FromComponentInNewPrefab(_bankPrefab)
                .UnderTransform(_gameplayContainer.transform)
                .AsSingle()
                .NonLazy();

            Container.Bind<Field>()
                .FromComponentInNewPrefab(_fieldPrefab)
                .UnderTransform(_gameplayContainer.transform)
                .AsSingle()
                .NonLazy();
        }

        private void LoadPlayerData()
        {
            _coins = YG2.saves.Coins;
            _shuffles = YG2.saves.Shuffles;
            _cleanings = YG2.saves.Cleanings;
            _cancels = YG2.saves.Cancels;
            _levelNumber = YG2.saves.Level;
        }

        private void BindPlayer()
        {
            Container.Bind<PlayerStats>().AsSingle().WithArguments(_coins, _shuffles, _cleanings, _cancels);
        }
    }
}