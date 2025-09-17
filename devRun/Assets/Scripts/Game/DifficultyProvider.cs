using UnityEngine;
using UniRx;

namespace Branches
{
    public class DifficultyProvider : MonoBehaviour
    {
        public static DifficultyProvider Instance { get; private set; }

        [SerializeField] private AnimationCurve _difficultyCurve;
        [SerializeField] private AnimationCurve _mergeEventIterations;

        [SerializeField] private int _maxDifficultyLevel = 10;

        private float GetDifficulty(int level)
        {
            var t = ((float)level - 1) / _maxDifficultyLevel;
            return Mathf.Clamp01(_difficultyCurve.Evaluate(t));
        }

        private int GetMergeEventIterations(int level)
        {
            var t = ((float)level - 1) / _maxDifficultyLevel;
            var result = _mergeEventIterations.Evaluate(t);
            result *= 10;
            return Mathf.RoundToInt(result);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Blackboard.Version.SkipLatestValueOnSubscribe().Subscribe(value =>
            {
                Blackboard.Difficulty.Value = GetDifficulty(value);
                Blackboard.MergeEventIterations.Value = GetMergeEventIterations(value);
                Debug.Log("Difficulty: " + Blackboard.Difficulty.Value);

                if (Blackboard.Version.Value >= Mathf.RoundToInt(_maxDifficultyLevel * .5f))
                {
                    Blackboard.MaxLanesCount.Value = 4;
                }

            }).AddTo(this);
        }
    }
}
