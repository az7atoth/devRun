using Az7.Utils.Pool;
using Az7.Utils.Timers;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Timer = Az7.Utils.Timers.Timer;

namespace Branches
{
    public class Collectable : MonoBehaviour
    {
        public static IObservable<Collectable> OnDeactivate => _onDeactivate.AsObservable();
        private static ReactiveCommand<Collectable> _onDeactivate = new();
        public static IObservable<Collectable> OnCollected => _onCollected.AsObservable();
        private static ReactiveCommand<Collectable> _onCollected = new();

        public CollectableType Type { get; private set; }
        public BranchesColorIndex ColorIndex { get; private set; }

        public bool IsCollected { get; private set; }

        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private PoolableObject _poolableObject;
        [SerializeField] private float _defaultAnimationTime = .2f;
        [SerializeField] private CollectableSetting[] _settings;

        private bool _hidedByDistance;
        private bool _isVisible;
        private CancellationTokenSource _cts;
        private CollectableSetting _activeSetting;
        private Timer _transitionTimer = new();

        public void Activate(CollectableType type)
        {
            if (!SetView(type))
            {
                Debug.Log("Failed to set collectable view! Deactivate " + gameObject.name);
                Deactivate(true);
            }

            Type = type;
            IsCollected = false;
            gameObject.SetActive(true);

            _hidedByDistance = false;
            _isVisible = true;
            _collider.enabled = true;
        }

        public void Deactivate(bool notify)
        {
            if (notify)
            {
                _onDeactivate.Execute(this);
            }

            if (_poolableObject != null)
            {
                _poolableObject.Return();
            }
            else
            {
                Debug.Log("Deactivate " + gameObject.name + " is failed. Destroy.");
                Destroy(gameObject);
            }
        }

        public void Collect()
        {
            _onCollected.Execute(this);
            IsCollected = true;
            Deactivate(true);
        }

        public void Show(float animationTime = 0f)
        {
            if (_hidedByDistance || _isVisible) { return; }

            if (animationTime == 0f)
            {
                animationTime = _defaultAnimationTime;
            }

            Cancel();
            _cts = new();
            ToggleViewAsync(true, animationTime, _cts.Token).Forget();
        }

        public void Hide(float animationTime = 0f)
        {
            if (!_isVisible) return;

            if (animationTime == 0f)
            {
                animationTime = _defaultAnimationTime;
            }

            Cancel();
            _cts = new();
            ToggleViewAsync(false, animationTime, _cts.Token).Forget();
        }

        public void HideByDistance(float animationTime = 0f)
        {
            _hidedByDistance = true;
            Hide(animationTime);
        }

        private async UniTaskVoid ToggleViewAsync(bool show, float animationTime, CancellationToken token)
        {
            if (!show)
            {
                _collider.enabled = false;
                _isVisible = false;
            }

            var iconStartColor = _activeSetting.Icon.color;
            var backgroundStartColor = _background.color;

            var iconEndColor = Color.clear;
            var backgroundEndColor = Color.clear;

            if (show)
            {
                iconEndColor = _activeSetting.IconColor;
                backgroundEndColor = _activeSetting.BackgroundColor;
            }

            _transitionTimer.Set(animationTime);
            _transitionTimer.Start();

            while (_transitionTimer.State != TimerState.Completed)
            {
                _activeSetting.Icon.color = Color.Lerp(iconStartColor, iconEndColor, _transitionTimer.Ratio);
                _background.color = Color.Lerp(backgroundStartColor, backgroundEndColor, _transitionTimer.Ratio);

                _transitionTimer.Update(Time.deltaTime);

                await UniTask.Yield();

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }

            if (show)
            {
                _collider.enabled = true;
                _isVisible = true;
            }
        }

        private bool SetView(CollectableType type)
        {
            CollectableSetting setting = null;

            for (int i = 0; i < _settings.Length; i++)
            {
                if (_settings[i].Type == type)
                {
                    setting = _settings[i];
                }
                else
                {
                    _settings[i].Icon.color = Color.clear;
                }
            }

            if (setting == null) return false;

            _activeSetting = setting;

            _activeSetting.BackgroundColor = ColorProvider.Instance.Get(_activeSetting.BackgroundColorIndex);
            _activeSetting.IconColor = ColorProvider.Instance.Get(_activeSetting.IconColorIndex);

            _background.color = _activeSetting.BackgroundColor;
            _activeSetting.Icon.color = _activeSetting.IconColor;

            if (type != CollectableType.Death)
            {
                ColorIndex = _activeSetting.BackgroundColorIndex;
            }
            else
            {
                ColorIndex = BranchesColorIndex.White;
            }

            return true;
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void OnDisable()
        {
            Cancel();
        }

        [Serializable]
        public class CollectableSetting
        {
            [field: SerializeField] public CollectableType Type { get; private set; }
            [field: SerializeField] public BranchesColorIndex IconColorIndex { get; private set; }
            [field: SerializeField] public BranchesColorIndex BackgroundColorIndex { get; private set; }
            [field: SerializeField] public SpriteRenderer Icon { get; private set; }

            public Color IconColor { get; set; }
            public Color BackgroundColor { get; set; }
        }
    }
}
