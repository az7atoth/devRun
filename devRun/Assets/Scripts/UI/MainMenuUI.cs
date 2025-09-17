using Az7.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Branches
{
    public class MainMenuUI : UI_ViewBase
    {
        public override UI_ViewKey ViewKey => UI_ViewKey.Main;

        [SerializeField] private Button _exit;

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _exit.gameObject.SetActive(false);
            }

            _exit.onClick.AsObservable().Subscribe(_ => { Application.Quit(); }).AddTo(this);
        }
    }
}
