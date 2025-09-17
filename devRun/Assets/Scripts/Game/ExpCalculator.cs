using UnityEngine;
using UniRx;

namespace Branches
{
    public class ExpCalculator : MonoBehaviour
    {
        [SerializeField] private int _startValue;
        [SerializeField] private float _y;
        [SerializeField] private int _levelTest;

        private int GetRequested(int level)
        {
            //l_exp =  x * (level ^ y) - (x * level)
            //l_exp = (level / x) ^ y
            if (level < 1) { return 0; }

            //return Mathf.RoundToInt(Mathf.Pow((float)level / _startValue, _y));
            return Mathf.RoundToInt(_startValue + _startValue * (level - 1));
        }

        private void Awake()
        {
            Blackboard.Version.Subscribe(value =>
            {
                Blackboard.CodeRequested.Value = GetRequested(value);
            }).AddTo(this);
        }

        //private void OnValidate()
        //{
        //    Debug.Log(GetRequested(_levelTest));
        //}
    }
}
