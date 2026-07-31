using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Bank _bankPrefab;
    [SerializeField] private Field _fieldPrefab;    
    
    public override void InstallBindings()
    {
        GameplayCanvas canvas = FindFirstObjectByType<GameplayCanvas>();

        Container.Bind<Bank>()
            .FromComponentInNewPrefab(_bankPrefab)
            .UnderTransform(canvas.transform)
            .AsSingle()
            .NonLazy();

        Container.Bind<Field>()
            .FromComponentInNewPrefab(_fieldPrefab)
            .UnderTransform(canvas.transform)
            .AsSingle()
            .NonLazy();

        Container.Bind<LevelState>().AsSingle().NonLazy();
        Container.Bind<Level>().AsSingle().NonLazy();
    }
}
