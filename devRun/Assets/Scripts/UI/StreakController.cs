using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

namespace DevRun
{
    public class StreakController : MonoBehaviour
    {
        public static StreakController Instance { get; private set; }

        [SerializeField] private TMP_Text _counterText;
        [SerializeField] private Image _fill;
        [SerializeField] private int[] _targets;

        public void Clear()
        {
            Blackboard.StreakModifier.Value = 0;
            Blackboard.StreakCount.SetValueAndForceNotify(0);
        }

        private void DefineValues(int streakCount)
        {
            if (Blackboard.StreakModifier.Value == _targets.Length) return;

            if (streakCount >= _targets[Blackboard.StreakModifier.Value])
            {
                if (Blackboard.StreakModifier.Value + 1 <= _targets.Length)
                {
                    Blackboard.StreakModifier.Value++;
                    _counterText.text = "x" + Blackboard.StreakModifier.Value.ToString();

                    Blackboard.StreakCount.SetValueAndForceNotify(0);
                }
            }
        }

        private void Update()
        {
            float fill;

            if (Blackboard.StreakModifier.Value == _targets.Length)
            {
                fill = 1f;
            }
            else
            {
                if (_targets[Blackboard.StreakModifier.Value] == 0)
                {
                    fill = 0f;
                }
                else
                {
                    fill = (float)Blackboard.StreakCount.Value / _targets[Blackboard.StreakModifier.Value];
                }
            }

            _fill.fillAmount = Mathf.Lerp(_fill.fillAmount, fill, Time.deltaTime * 10f);
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

            Blackboard.StreakCount.Subscribe(count =>
            {
                DefineValues(count);
            }).AddTo(this);
        }
    }
}
