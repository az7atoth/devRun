using Az7.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;
using Timer = Az7.Utils.Timers.Timer;

namespace DevRun
{
    public class MergeEventController : MonoBehaviour
    {
        public static MergeEventController Instance { get; private set; }

        [SerializeField] private float _transitionDuration = .3f;
        [SerializeField] private Transform _cameraTarget;

        private MergeUI _mergeUI;
        private PerkChooseUI _chooseUI;
        private CancellationTokenSource _cts;
        private Timer _transitionTimer = new();
        private Branch _branch;

        private UI_Controller _uI_Controller;
        private ICollectablesSpawner _collectablesSpawner;

        [Inject]
        public void Construct(UI_Controller uI_Controller, ICollectablesSpawner collectablesSpawner)
        {
            _uI_Controller = uI_Controller;
            _collectablesSpawner = collectablesSpawner;
        }

        public void Activate()
        {
            if (_mergeUI == null)
            {
                _mergeUI = _uI_Controller.GetView(UI_ViewKey.Merge) as MergeUI;
            }

            if (_chooseUI == null)
            {
                _chooseUI = _uI_Controller.GetView(UI_ViewKey.PerkChoose) as PerkChooseUI;
            }

            _cts = new();
            ActivateAsync(_cts.Token).Forget();
        }

        public void DeactivateImmidiate()
        {
            Cancel();
        }

        private async UniTaskVoid ActivateAsync(CancellationToken token)
        {
            _mergeUI.Prepare();

            _collectablesSpawner.HideAll(_transitionDuration);

            await UniTask.WhenAll(
                GameSpeedController.Instance.SetRatioAsync(.1f, _transitionDuration, token),
                _uI_Controller.ShowViewAsync(UI_ViewKey.Merge, _transitionDuration, token),
                MixingCameraController.Instance.ToggleCameraAsync(false, _transitionDuration, token),
                AnimateBranchAsync(_transitionDuration, token)
                );

            if (token.IsCancellationRequested)
            {
                //
                return;
            }

            _mergeUI.Activate();

            while (!_mergeUI.EventOvered)
            {
                _cameraTarget.position = _branch.EndPosition;

                await UniTask.Yield();

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }

            _mergeUI.Deactivate();

            //TODO move to handler
            if (_mergeUI.EventResult)
            {
                Blackboard.CodeStored.Value += Blackboard.CodeCollected.Value;
                Blackboard.CodeCollected.Value = 0;
            }
            else
            {
                Blackboard.CodeCollected.Value = 0;
            }

            if (Blackboard.CodeStored.Value >= Blackboard.CodeRequested.Value)
            {
                Blackboard.OnVersionUpgrade.Execute();

                Blackboard.Version.Value++;
                Blackboard.CodeStored.Value = 0;

                Blackboard.GameSpeedRatio.Value = 0f;

                _chooseUI.Setup();
                await _chooseUI.ShowAsync(.2f, token);
                if (token.IsCancellationRequested) { return; }

                _chooseUI.Activate();

                await UniTask.WaitUntil(() => _chooseUI.PerkSelected, cancellationToken: token).SuppressCancellationThrow();
                if (token.IsCancellationRequested) { return; }

                _chooseUI.HideAsync(.2f, token).Forget();
            }

            _collectablesSpawner.ShowAll(_transitionDuration * 1.2f);

            await UniTask.WhenAll(
                _mergeUI.HideAsync(_transitionDuration, token),
                MixingCameraController.Instance.ToggleCameraAsync(true, _transitionDuration, token)
                );

            if (token.IsCancellationRequested) { return; }

            _cameraTarget.transform.position = PositionProvider.Instance.MainBranchStart.position;
            PlayerController.Instance.InMergeEvent = false;

            await GameSpeedController.Instance.SetRatioAsync(1f, _transitionDuration * 1.2f, token);
        }

        private async UniTask AnimateBranchAsync(float transitionTime, CancellationToken token)
        {
            _branch = BranchController.Instance.Get();
            _branch.Activate(PlayerController.Instance.transform.position);
            _branch.IsMoving = false;
            _cameraTarget.position = PlayerController.Instance.transform.position;

            var startPosition = PlayerController.Instance.transform.position;

            var distanceX = Vector3.Distance(startPosition, PositionProvider.Instance.MainBranchStart.transform.position);
            var distanceY = distanceX * (Mathf.Sin(30 * Mathf.Deg2Rad) / Mathf.Sin(60 * Mathf.Deg2Rad));

            var endPosition = PositionProvider.Instance.MainBranchStart.transform.up * distanceY;
            var position = startPosition;

            _transitionTimer.Set(transitionTime);
            _transitionTimer.Start();

            while (_transitionTimer.State != Az7.Utils.Timers.TimerState.Completed)
            {
                position = Vector3.Lerp(startPosition, endPosition, _transitionTimer.Ratio);
                _branch.SetEnd(position);
                _cameraTarget.transform.position = position;

                _transitionTimer.Update(Time.deltaTime);

                await UniTask.Yield();

                if (token.IsCancellationRequested)
                {
                    _transitionTimer.Stop();
                    return;
                }
            }

            _branch.SetEnd(endPosition);
            _branch.LeaveImmidiate(endPosition);
            _branch.IsMoving = true;
            _cameraTarget.transform.position = endPosition;
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
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
        }

        private void OnDestroy()
        {
            Cancel();
        }
    }
}
