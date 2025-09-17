using System;
using UniRx;

namespace Az7.Utils.Timers
{
    public enum TimerState
    {
        Stopped = 0,
        Started = 1,
        Completed = 2,
    }

    public class Timer
    {
        /// <summary>
        /// Value between 0 and 1
        /// </summary>
        public float Ratio => GetRatioClamped();
        public TimerState State { get; protected set; } = TimerState.Stopped;

        protected float _duration;
        protected float _time;

        public Timer() { }

        public Timer(float duration)
        {
            Set(duration);
        }

        public void Set(float duration)
        {
            if (duration < 0f)
            {
                duration = 0f;
            }

            _duration = duration;
        }

        public virtual void Start()
        {
            _time = 0f;
            State = TimerState.Started;
        }

        public virtual void Stop()
        {
            State = TimerState.Stopped;
        }

        protected virtual void Complete()
        {
            State = TimerState.Completed;
        }

        public virtual void Update(float deltaTime)
        {
            if (State == TimerState.Started)
            {
                _time += deltaTime;

                if (_time >= _duration)
                {
                    Complete();
                }
            }
        }

        protected float GetRatioClamped()
        {
            if (_duration == 0f) return 0f;

            var value = _time / _duration;
            if (value < 0f)
            {
                return 0f;
            }
            else if (value > 1f)
            {
                return 1f;
            }
            else
            {
                return value;
            }
        }
    }

    public class ReactiveTimer : Timer, IDisposable
    {
        public IObservable<Unit> OnStart => _onStart.AsObservable();
        public IObservable<Unit> OnUpdate => _onStart.AsObservable();
        public IObservable<Unit> OnComplete => _onStart.AsObservable();

        private ReactiveCommand _onStart = new();
        private ReactiveCommand _onUpdate = new();
        private ReactiveCommand _onComplete = new();

        public override void Start()
        {
            base.Start();
            _onStart.Execute();
        }

        protected override void Complete()
        {
            base.Complete();
            _onComplete.Execute();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (State == TimerState.Started)
            {
                _onUpdate.Execute();
            }
        }

        public void Dispose()
        {
            _onStart.Dispose();
            _onUpdate.Dispose();
            _onComplete.Dispose();
        }
    }
}
