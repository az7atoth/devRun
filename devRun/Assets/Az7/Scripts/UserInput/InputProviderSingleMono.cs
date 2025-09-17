using System;
using UnityEngine;

namespace Az7.UserInput
{
    public class InputProviderSingleMono : MonoBehaviour, IInputProvider
    {
        public static InputProviderSingleMono Instance { get; private set; }

        public IObservable<bool> OnMoveLeft => _inputProvider.OnMoveLeft;

        public IObservable<bool> OnMoveRight => _inputProvider.OnMoveRight;

        public IObservable<bool> OnAction => _inputProvider.OnAction;


        [SerializeField] private bool _initializeOnAwake = true;
        private InputProvider _inputProvider;

        public void Initialize()
        {
            if (_inputProvider != null) return;

            _inputProvider = new InputProvider();
            _inputProvider.Initialize();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Awake()
        {
            if (_initializeOnAwake)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            _inputProvider?.Dispose();
        }
    }
}
