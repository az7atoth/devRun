using DevRun;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerController _playerController;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerController>().FromInstance(_playerController);
    }
}