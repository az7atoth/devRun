using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Az7.UI
{
    public class UI_Controller : MonoBehaviour
    {
        [SerializeField] private bool _initializeOnAwake = true;

        private Dictionary<UI_ViewKey, UI_ViewBase> _views = new();
        private bool _isInitialized = false;

        public void Initialize()
        {
            if (_isInitialized) return;

            var views = GetComponentsInChildren<UI_ViewBase>();

            foreach (var view in views)
            {
                _views.Add(view.ViewKey, view);
                view.HideImmidiate();
            }

            _isInitialized = true;
        }

        public void ShowView(UI_ViewKey key)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && !view.IsVisible)
            {
                view.ShowImmidiate();
                Debug.Log("<color=#65D766>UI</color> Show: " + key);
            }
        }

        public void HideView(UI_ViewKey key)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && view.IsVisible)
            {
                view.HideImmidiate();
                Debug.Log("<color=#65D766>UI</color> Hide: " + key);
            }
        }

        public async UniTask ShowViewAsync(UI_ViewKey key, CancellationToken token)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && !view.IsVisible)
            {
                Debug.Log("<color=#65D766>UI</color> Start show Async: " + key);
                await view.ShowAsync(token);
            }
            else
            {
                Debug.Log("<color=#65D766>UI</color> View not found: " + key);
                return;
            }
        }

        public async UniTask ShowViewAsync(UI_ViewKey key, float duration, CancellationToken token)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && !view.IsVisible)
            {
                Debug.Log("<color=#65D766>UI</color> Start show Async: " + key);
                await view.ShowAsync(duration, token);
            }
            else
            {
                Debug.Log("<color=#65D766>UI</color> View not found: " + key);
                return;
            }
        }

        public async UniTask HideViewAsync(UI_ViewKey key, CancellationToken token)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && !view.IsVisible)
            {
                Debug.Log("<color=#65D766>UI</color> Start hide Async: " + key);
                await view.HideAsync(token);
            }
            else
            {
                Debug.Log("<color=#65D766>UI</color> View not found: " + key);
                return;
            }
        }

        public async UniTask HideViewAsync(UI_ViewKey key, float duration, CancellationToken token)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view) && !view.IsVisible)
            {
                Debug.Log("<color=#65D766>UI</color> Start hide Async: " + key);
                await view.HideAsync(duration, token);
            }
            else
            {
                Debug.Log("<color=#65D766>UI</color> View not found: " + key);
                return;
            }
        }

        public void HideAll()
        {
            foreach (var view in _views)
            {
                if (view.Value.IsVisible && !view.Value.IgnoreHideAll)
                {
                    view.Value.HideImmidiate();
                    Debug.Log("<color=#65D766>UI</color> Hide: " + view.Key);
                }
            }
        }

        public UI_ViewBase GetView(UI_ViewKey key)
        {
            if (_views.TryGetValue(key, out UI_ViewBase view))
            {
                return view;
            }
            else
            {
                return null;
            }
        }

        protected virtual void AwakeBase() { }

        private void Awake()
        {
            AwakeBase();

            if (_initializeOnAwake)
            {
                Initialize();
            }
        }
    }
}
