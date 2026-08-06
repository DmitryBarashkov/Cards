using UnityEngine;
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
    [SerializeField] private UIScreen _shopGameScreen;

    [Header("Контейнеры для экранов")]
    [SerializeField] private Transform _endGameContainer;
    [SerializeField] private Transform _shopContainer;
    [SerializeField] private GameplayContainer _gameplayContainer;

    private int _coins;
    private int _shuffles;
    private int _cleanings;
    private int _cancels;

    public override void InstallBindings()
    {
        BindServices();
        BindLevel();
        BindGameObjects();

        LoadPlayerData();
        BindPlayer();
    }

    private void BindServices()
    {
        
        Container.BindInterfacesAndSelfTo<ShopService>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<UIService>()
            .AsSingle()
            .WithArguments(_winGameScreen, _loseGameScreen, _shopGameScreen, _endGameContainer, _shopContainer)
            .NonLazy();

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
