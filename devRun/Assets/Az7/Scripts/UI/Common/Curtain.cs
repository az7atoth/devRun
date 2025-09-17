using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Az7.UI
{
    public class Curtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _defaultAnimationTime = 1.0f;
        [SerializeField] private bool _showByDefault;

        public void ShowImmidiate()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
        }

        public void HideImmidiate()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
        }

        public async UniTask ShowAsync(CancellationToken token)
        {
            await CurtainAnimationAsync(true, _defaultAnimationTime, token);
        }

        public async UniTask ShowAsync(float animationTime, CancellationToken token)
        {
            await CurtainAnimationAsync(true, animationTime, token);
        }

        public async UniTask HideAsync(CancellationToken token)
        {
            await CurtainAnimationAsync(false, _defaultAnimationTime, token);
        }

        public async UniTask HideAsync(float animationTime, CancellationToken token)
        {
            await CurtainAnimationAsync(false, animationTime, token);
        }

        private async UniTask CurtainAnimationAsync(bool show, float animationTime, CancellationToken token)
        {
            if (show)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            var startValue = _canvasGroup.alpha;
            var endValue = show ? 1f : 0f;

            if (endValue == startValue)
            {
                return;
            }

            if (animationTime < 0f)
            {
                animationTime = 0f;
            }

            var t = 0f;
            var time = 0f;

            while (t < 1f)
            {
                t = time / animationTime;
                time += Time.unscaledDeltaTime;

                _canvasGroup.alpha = Mathf.Lerp(startValue, endValue, t);

                await UniTask.Yield();
                if (token.IsCancellationRequested) return;
            }

            if (!show)
            {
                _canvasGroup.blocksRaycasts = false;
            }
        }

        protected virtual void AwakeBase() { }

        private void Awake()
        {
            AwakeBase();

            if (_showByDefault)
            {
                ShowImmidiate();
            }
            else
            {
                HideImmidiate();
            }
        }
    }
}