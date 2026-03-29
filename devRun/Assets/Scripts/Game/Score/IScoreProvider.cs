using UniRx;

namespace DevRun
{
    public interface IScoreProvider
    {
        public IReadOnlyReactiveProperty<int> ScoreCollected { get; }
        public IReadOnlyReactiveProperty<int> ScoreStored { get; }
        public IReadOnlyReactiveProperty<int> ScoreRequested { get; }
    }
}
