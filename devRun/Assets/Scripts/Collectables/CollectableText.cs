using Az7.Utils.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using Timer = Az7.Utils.Timers.Timer;

namespace DevRun
{
    public class CollectableText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private PoolableObject _poolableObject;

        private Timer _timer = new();
        private bool _isActive;

        public void Animate(string text, float duration, BranchesColorIndex colorIndex)
        {
            _text.text = text;
            _text.color = ColorProvider.Instance.Get(colorIndex);

            gameObject.SetActive(true);
            _isActive = true;

            AnimateAsync(duration, destroyCancellationToken).Forget();
        }

        private async UniTaskVoid AnimateAsync(float duration, CancellationToken token)
        {
            await UniTask.WaitForSeconds(duration * .5f, cancellationToken: token).SuppressCancellationThrow();
            if (token.IsCancellationRequested) return;

            _timer.Set(duration * .5f);
            _timer.Start();

            var startColor = _text.color;

            while (_timer.State != Az7.Utils.Timers.TimerState.Completed)
            {
                _text.color = Color.Lerp(startColor, Color.clear, _timer.Ratio);
                _timer.Update(Time.deltaTime);

                await UniTask.Yield();
                if (token.IsCancellationRequested) { return; }
            }

            _text.color = Color.clear;
            _isActive = false;
            _poolableObject.Return();
        }

        private void Update()
        {
            if (!_isActive) { return; }

            transform.position -= Vector3.down * Time.deltaTime
                * Blackboard.MovementSpeed.Value
                * Blackboard.GameSpeedRatio.Value
                * Blackboard.GameSpeedRatioModifier.Value
                * .2f;
        }
    }
}
