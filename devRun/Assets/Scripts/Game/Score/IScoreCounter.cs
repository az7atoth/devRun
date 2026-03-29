
namespace DevRun
{
    public interface IScoreCounter
    {
        public void ClearCollected();
        public void ClearStored();
        public void CollectedToStored();
        public void UpdateRequestedScore(int level);
        public void Add(int score);
        public void Subtract(int score);
    }
}
