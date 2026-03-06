using Az7.UI;
using UnityEngine;
using Zenject;

public class UI_Installer : MonoInstaller
{
    [SerializeField] private UI_Controller _uI_Controller;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<UI_Controller>().FromInstance(_uI_Controller).AsSingle();
    }
}