using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Bank _bankPrefab;
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private Transform _UICanvas;
    
    public override void InstallBindings()
    {
        Container.Bind<Bank>()
            .FromComponentInNewPrefab(_bankPrefab)
            .UnderTransform(_UICanvas)
            .AsSingle()
            .NonLazy();

        Container.Bind<Field>()
            .FromComponentInNewPrefab(_fieldPrefab)
            .UnderTransform(_UICanvas)
            .AsSingle()
            .NonLazy();

        Container.Bind<LevelState>().AsSingle().NonLazy();
        Container.Bind<Level>().AsSingle().NonLazy();
    }
}
