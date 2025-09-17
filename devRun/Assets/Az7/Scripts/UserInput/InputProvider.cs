using Az7.Extensions;
using Az7.Utils.Disposables;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Az7.UserInput
{
    public class InputProvider : IInputProvider, IInitializable, IDisposable
    {
        public bool IsInitialized { get; private set; }

        public IObservable<bool> OnMoveLeft { get; private set; }

        public IObservable<bool> OnMoveRight { get; private set; }

        public IObservable<bool> OnAction { get; private set; }


        private InputSystem_Actions _actions;
        private DisposableTracker _disposableTracker;

        public void Initialize()
        {
            if (IsInitialized) return;

            _disposableTracker = new();
            _actions = new();
            _actions.Enable();

            OnMoveLeft = _actions.Branches.MoveLeft.ObserveEveryValueChanged(x => x.ReadValue<float>() > 0);
            OnMoveRight = _actions.Branches.MoveRight.ObserveEveryValueChanged(x => x.ReadValue<float>() > 0);
            OnAction = _actions.Branches.Action.ObserveEveryValueChanged(x => x.ReadValue<float>() > 0);

            IsInitialized = true;
        }

        public void Dispose()
        {
            _actions.Dispose();
            _disposableTracker.Dispose();
        }
    }
}
