using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [Header("Настройки уровня")]
    [SerializeField] private int _totalTriplets = 20;    
    [SerializeField] private int _uniqueTypesCount = 5;
    [SerializeField] private int _bankSize = 7;

    [Header("Настройки сетки")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private int _maxLayers = 4;

    [Header("Карточки")]
    [SerializeField] private Card _cardPrefab;    

    public override void InstallBindings()
    {
        Container.Bind<CardFactory>().AsSingle().WithArguments(_cardPrefab);
        Container.Bind<CardNode>().AsTransient();
        Container.Bind<LevelGenerator>()
            .AsSingle()
            .WithArguments(_totalTriplets, _uniqueTypesCount, _bankSize,
                           _gridWidth, _gridHeight, _maxLayers);
    }
}
