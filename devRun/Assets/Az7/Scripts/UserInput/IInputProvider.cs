using System;
using UnityEngine;

namespace Az7.UserInput
{
    public interface IInputProvider
    {
        public IObservable<bool> OnMoveLeft { get; }
        public IObservable<bool> OnMoveRight { get; }
        public IObservable<bool> OnAction { get; }
    }
}
