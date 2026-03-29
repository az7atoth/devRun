using UniRx;
using UnityEngine;

namespace DevRun
{
    public class ScoreService : IScoreCounter, IScoreProvider
    {
        public IReadOnlyReactiveProperty<int> ScoreCollected => _scoreCollected;
        public IReadOnlyReactiveProperty<int> ScoreStored => _scoreStored;
        public IReadOnlyReactiveProperty<int> ScoreRequested => _scoreRequested;

        private ReactiveProperty<int> _scoreCollected = new();
        private ReactiveProperty<int> _scoreStored = new();
        private ReactiveProperty<int> _scoreRequested = new();

        public void Add(int score)
        {
            if (score <= 0) return;
            _scoreCollected.Value += score;
        }
        public void Subtract(int score)
        {
            if (score <= 0) return;
            var newValue = _scoreCollected.Value - score;
            if (newValue < 0) newValue = 0;
            _scoreCollected.Value -= newValue;
        }

        public void ClearCollected()
        {
            _scoreCollected.SetValueAndForceNotify(0);
        }

        public void ClearStored()
        {
            _scoreStored.SetValueAndForceNotify(0);
        }

        public void CollectedToStored()
        {
            _scoreStored.Value += _scoreCollected.Value;
            ClearCollected();
        }

        public void UpdateRequestedScore(int level)
        {
            _scoreRequested.Value = GetRequested(level);
        }

        private int GetRequested(int level)
        {
            var required = Mathf.RoundToInt(120 + Mathf.Pow(level, 1.6f) * 90);
            var rem = required % 10;
            required -= rem;
            return required;
        }
    }
}
