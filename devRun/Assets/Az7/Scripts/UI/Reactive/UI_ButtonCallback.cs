using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Az7.UI
{
    public class UI_ButtonCallback : MonoBehaviour
    {
        public static IObservable<UI_ButtonKey> Callback => _command.AsObservable();
        private static ReactiveCommand<UI_ButtonKey> _command = new();

        [SerializeField] private Button _button;
        [SerializeField] private UI_ButtonKey _callbackKey;
        private IDisposable _disposable;

        private void OnEnable()
        {
            _disposable = _button.onClick.AsObservable().Subscribe(_ => _command.Execute(_callbackKey));
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}
