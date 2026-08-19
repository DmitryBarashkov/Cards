using System;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Префабы объектов")]
    [SerializeField] private Bank _bankPrefab;
    [SerializeField] private Field _fieldPrefab;

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

    private int _coins;
    private int _shuffles;
    private int _cleanings;
    private int _cancels;

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
        Container.Bind<LevelState>().AsSingle().NonLazy();
        Container.Bind<Level>()
            .AsSingle()
            .WithArguments(YG2.saves.level)
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
        _coins = YG2.saves.coins;
        _shuffles = YG2.saves.shuffles;
        _cleanings = YG2.saves.cleanings;
        _cancels = YG2.saves.cancels;
    }

    private void BindPlayer()
    {
        Container.Bind<PlayerStats>().AsSingle().WithArguments(_coins, _shuffles, _cleanings, _cancels);
    }
}
