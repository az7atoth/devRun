using DevRun;
using UnityEngine;
using Zenject;

public class CollectablesInstaller : MonoInstaller
{
    [SerializeField] private PatternBasedSpawner _collectablesSpawner;
    [SerializeField] private SpawnPatternProvider _spawnPatternProvider;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ICollectablesSpawner>().FromInstance(_collectablesSpawner).AsSingle();
        Container.BindInterfacesAndSelfTo<SpawnPatternProvider>().FromInstance(_spawnPatternProvider).AsSingle();
    }
}