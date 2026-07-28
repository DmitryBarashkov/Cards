using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private AudioService _audioServicePrefab;
    [SerializeField] private CardsDatabase _cardsDatabase;
    
    public override void InstallBindings()
    {
        Container.Bind<CardsDatabase>().FromInstance(_cardsDatabase).AsSingle().NonLazy();
        Container.Bind<InputService>().AsSingle().NonLazy();
        
        Container.BindInterfacesAndSelfTo<AudioService>()
            .FromComponentInNewPrefab(_audioServicePrefab)
            .UnderTransformGroup("GlobalServices")
            .AsSingle()
            .NonLazy();
    }
}
