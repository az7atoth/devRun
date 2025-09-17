using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Az7.UI
{
    public abstract class UI_ViewBase : MonoBehaviour
    {
        public abstract UI_ViewKey ViewKey { get; }
        public bool IsVisible { get; protected set; }

        [field: SerializeField] public float DefaultAnimationTime = 1f;
        [field: SerializeField] public bool IgnoreHideAll { get; set; } = false;
        [SerializeField] protected CanvasGroup _canvasGroup;

        public virtual void ShowImmidiate()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
            IsVisible = true;
        }

        public virtual void HideImmidiate()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
            IsVisible = false;
        }

        public virtual async UniTask ShowAsync(CancellationToken token)
        {
            await CurtainAnimationAsync(true, DefaultAnimationTime, token);
        }

        public virtual async UniTask ShowAsync(float animationTime, CancellationToken token)
        {
            await CurtainAnimationAsync(true, animationTime, token);
        }

        public virtual async UniTask HideAsync(CancellationToken token)
        {
            await CurtainAnimationAsync(false, DefaultAnimationTime, token);
        }

        public virtual async UniTask HideAsync(float animationTime, CancellationToken token)
        {
            await CurtainAnimationAsync(false, animationTime, token);
        }

        private async UniTask CurtainAnimationAsync(bool show, float animationTime, CancellationToken token)
        {
            if (show)
            {
                _canvasGroup.blocksRaycasts = true;
                IsVisible = true;
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
                IsVisible = false;
            }
        }
    }
}
