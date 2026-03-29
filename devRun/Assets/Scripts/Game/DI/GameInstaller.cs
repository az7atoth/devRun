using UnityEngine;
using Zenject;

namespace DevRun
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        }
    }
}