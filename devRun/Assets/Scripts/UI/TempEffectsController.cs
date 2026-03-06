using Az7.Utils.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace DevRun
{
    public class TempEffectsController : MonoBehaviour
    {
        public static TempEffectsController Instance { get; private set; }

        [SerializeField] private TempEffectView[] _views;
        [SerializeField] private Dictionary<CollectableType, TempEffectView> _viewsDic = new();
        [SerializeField] private Dictionary<CollectableType, Timer> _timersDic = new();
        [SerializeField] private float _defaultEffectDuration;

        [Space]
        [SerializeField] private float _cofeeRatioMod = .5f;
        [SerializeField] private float _cofeeTransitionTime = .5f;
        [Space]
        [SerializeField] private float _energyTransitionSpeedModifier = 1.3f;
        [SerializeField] private float _speedVirusSpeedModifier = 0.8f;

        public void Activate(CollectableType type)
        {
            if (!_viewsDic.TryGetValue(type, out var view)) return;

            var time = _defaultEffectDuration;

            if (type == CollectableType.Cofee || type == CollectableType.Energy || type == CollectableType.Shield)
            {
                time *= Blackboard.BuffTimeModifier.Value;
            }
            else
            {
                time *= Blackboard.DebuffTimeModifier.Value;
            }

            Debug.Log(time);

            _viewsDic[type].gameObject.SetActive(true);
            _timersDic[type].Set(time);
            _timersDic[type].Start();

            ApplyEffect(type, false);
        }

        public void ClearAll()
        {
            foreach (var item in _viewsDic)
            {
                item.Value.gameObject.SetActive(false);
            }

            foreach (var item in _timersDic)
            {
                item.Value.Stop();
            }
        }

        private void ApplyEffect(CollectableType type, bool cancel)
        {
            switch (type)
            {
                case CollectableType.Cofee:
                    if (cancel)
                    {
                        GameSpeedController.Instance.SetRatioModifier(1f, _cofeeTransitionTime);
                    }
                    else
                    {
                        GameSpeedController.Instance.SetRatioModifier(_cofeeRatioMod, _cofeeTransitionTime);
                    }
                    break;

                case CollectableType.Energy:
                    if (cancel)
                    {
                        Blackboard.TransitionSpeedModifier.Value = 1f;
                    }
                    else
                    {
                        if (_timersDic[CollectableType.TransitionSpeedVirus].State == TimerState.Started)
                        {
                            _timersDic[CollectableType.TransitionSpeedVirus].Stop();
                            _viewsDic[CollectableType.TransitionSpeedVirus].gameObject.SetActive(false);
                        }

                        Blackboard.TransitionSpeedModifier.Value = _energyTransitionSpeedModifier;
                    }
                    break;

                case CollectableType.Shield:
                    PlayerController.Instance.IsShielded = !cancel;
                    break;

                case CollectableType.ControlVirus:
                    PlayerController.Instance.SwapControls = !cancel;
                    break;

                case CollectableType.TransitionSpeedVirus:
                    if (cancel)
                    {
                        Blackboard.TransitionSpeedModifier.Value = 1f;
                    }
                    else
                    {
                        if (_timersDic[CollectableType.Energy].State == TimerState.Started)
                        {
                            _timersDic[CollectableType.Energy].Stop();
                            _viewsDic[CollectableType.Energy].gameObject.SetActive(false);
                        }

                        Blackboard.TransitionSpeedModifier.Value = _speedVirusSpeedModifier;
                    }
                    break;

                case CollectableType.MergeLockVirus:
                    PlayerController.Instance.MergeLock = !cancel;
                    break;
            }
        }

        private void Update()
        {
            if (Blackboard.GameState.Value != GameState.Running) return;

            foreach (var item in _timersDic)
            {
                if (item.Value.State == TimerState.Started)
                {
                    _viewsDic[item.Key].Fill.fillAmount = 1f - item.Value.Ratio;
                    item.Value.Update(Time.deltaTime * Blackboard.GameSpeedRatio.Value);
                }
                else if (item.Value.State == TimerState.Completed)
                {
                    item.Value.Stop();
                    _viewsDic[item.Key].gameObject.SetActive(false);
                    ApplyEffect(item.Key, true);
                }
            }
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

            foreach (var view in _views)
            {
                _viewsDic.Add(view.Type, view);
                _timersDic.Add(view.Type, new Timer());
            }
        }
    }
}
