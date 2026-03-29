using UnityEngine;
using Zenject;

namespace DevRun
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private PlayerController _playerController;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().FromInstance(_playerController);
        }
    } 
}