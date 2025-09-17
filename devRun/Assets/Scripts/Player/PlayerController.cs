using Az7.Extensions;
using Az7.UserInput;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Az7.Utils.Pool;
using System.Threading.Tasks;

namespace Branches
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        public bool InTransition { get; private set; }
        public bool InMergeEvent { get; set; }
        public bool SwapControls { get; set; }
        public bool IsShielded { get; set; }
        public bool MergeLock { get; set; }

        [SerializeField] private Pool _textPool;

        private bool _isInitialized;
        private Branch _currentBranch;
        private CancellationTokenSource _cts;
        private TransitionDirection _transitionDirection;

        public void Initialize()
        {
            if (_isInitialized) return;

            InputProviderSingleMono.Instance.OnMoveLeft.Subscribe(pressed => MoveLeft(pressed)).AddTo(this);
            InputProviderSingleMono.Instance.OnMoveRight.Subscribe(pressed => MoveRight(pressed)).AddTo(this);
            InputProviderSingleMono.Instance.OnAction.Subscribe(pressed => OnAction(pressed)).AddTo(this);
        }

        public void Setup()
        {
            if (_currentBranch != null)
            {
                _currentBranch.Deactivate();
                _currentBranch = null;
            }

            transform.position = PositionProvider.Instance.MainBranchStart.position;
        }

        private void OnAction(bool pressed)
        {
            if (!pressed
                || MergeLock
                || InTransition
                || InMergeEvent
                || Blackboard.CodeCollected.Value == 0
                || Blackboard.GameState.Value != GameState.Running) { return; }

            InMergeEvent = true;
            StreakController.Instance.Clear();
            MergeEventController.Instance.Activate();
        }

        public void MoveRight(bool pressed)
        {
            if (InMergeEvent) { return; }

            if (!pressed)
            {
                _transitionDirection = TransitionDirection.None;
            }
            else
            {
                if (SwapControls)
                {
                    _transitionDirection = TransitionDirection.Left;
                }
                else
                {
                    _transitionDirection = TransitionDirection.Right;
                }
            }

            if (InTransition || Blackboard.GameState.Value != GameState.Running) { return; }

            TryMove();
        }

        public void MoveLeft(bool pressed)
        {
            if (InMergeEvent) { return; }

            if (!pressed)
            {
                _transitionDirection = TransitionDirection.None;
            }
            else
            {
                if (SwapControls)
                {
                    _transitionDirection = TransitionDirection.Right;
                }
                else
                {
                    _transitionDirection = TransitionDirection.Left;
                }
            }

            if (InTransition || Blackboard.GameState.Value != GameState.Running) { return; }

            TryMove();
        }

        private bool TryMove()
        {
            var result = CheckForMovePosition(_transitionDirection, out var targetPosition);

            if (result)
            {
                _cts?.Dispose();
                _cts = new();
                MoveTransitionAsync(_transitionDirection, targetPosition, _cts.Token).Forget();
            }

            return result;
        }

        private async UniTaskVoid MoveTransitionAsync(TransitionDirection direction, Vector3 targetPosition, CancellationToken token)
        {
            InTransition = true;

            if (_currentBranch != null)
            {
                _currentBranch.Leave(transform.position);
            }

            _currentBranch = BranchController.Instance.Get();
            _currentBranch.Activate(transform.position);

            do
            {
                var isLeft = targetPosition.x < transform.position.x;

                while (
                    !Mathf.Approximately(transform.position.x, targetPosition.x)
                    &&
                    ((isLeft && transform.position.x > targetPosition.x)
                    || (!isLeft && transform.position.x < targetPosition.x))
                    )
                {
                    transform.position += (isLeft ? Vector3.left : Vector3.right)
                        * Time.deltaTime * Blackboard.MovementSpeed.Value
                        * Blackboard.GameSpeedRatio.Value
                        * Blackboard.GameSpeedRatioModifier.Value
                        * Blackboard.TransitionSpeed.Value
                        * Blackboard.TransitionSpeedModifier.Value
                        * Blackboard.PerkTransitionSpeedModifier.Value;

                    _currentBranch.SetEnd(transform.position);

                    await UniTask.Yield();

                    if (token.IsCancellationRequested)
                    {
                        InTransition = false;
                        return;
                    }
                }

                transform.position = new Vector3(targetPosition.x, 0f, 0f);

            } while (_transitionDirection == direction
                //&& (_transitionDirection == TransitionDirection.Left || _transitionDirection == TransitionDirection.Right)
                && CheckForMovePosition(_transitionDirection, out targetPosition));

            _currentBranch.SetKnee(transform.position);

            if (_transitionDirection != TransitionDirection.None)
            {
                if (TryMove())
                {
                    return;
                }
            }

            InTransition = false;
            Cancel();
        }

        private bool CheckForMovePosition(TransitionDirection direction, out Vector3 targetPosition)
        {
            targetPosition = Vector3.zero;

            if (_transitionDirection == TransitionDirection.None)
            {
                return false;
            }

            targetPosition = transform.position
                + (direction == TransitionDirection.Left ? Vector3.left : Vector3.right)
                * PositionProvider.Instance.LaneStep;

            var minXPosition = PositionProvider.Instance.MainBranchStart.position.x
                + PositionProvider.Instance.LaneStep;

            var maxXPosition = PositionProvider.Instance.MainBranchStart.position.x
                + PositionProvider.Instance.LaneStep * Blackboard.MaxLanesCount.Value;

            return targetPosition.x >= minXPosition && targetPosition.x <= maxXPosition;
        }

        private void Update()
        {
            if (_currentBranch != null)
            {
                _currentBranch.SetEnd(transform.position);
            }
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

            Blackboard.GameState.Subscribe(state =>
            {
                if (state == GameState.Overed)
                {
                    Cancel();
                }
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            Cancel();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (InMergeEvent) { return; }

            if (collision.gameObject.CompareTag("Collectable"))
            {
                if (collision.gameObject.TryGetComponent<Collectable>(out var collectable))
                {
                    switch (collectable.Type)
                    {
                        case CollectableType.Bug:
                            if (IsShielded) break;
                            Blackboard.BugsCollected.Value++;
                            OnNegativeEffect(collectable);
                            break;

                        case CollectableType.Fix:
                            var value = Blackboard.BugsCollected.Value - 1;
                            if (value < 0) value = 0;
                            Blackboard.BugsCollected.Value = value;
                            Blackboard.OnBuffPickUp.Execute();
                            break;

                        case CollectableType.Code:
                            Blackboard.CodeCollected.Value += 1 * Blackboard.StreakModifier.Value;
                            Blackboard.StreakCount.Value++;
                            var text = _textPool.Take().GetComponent<CollectableText>();
                            text.transform.position = collectable.transform.position;// + Vector3.up * .5f;
                            text.Animate("+" + (1 * Blackboard.StreakModifier.Value).ToString(), 1f, BranchesColorIndex.Yellow);
                            Blackboard.OnGoodPickUp.Execute();
                            break;

                        case CollectableType.Death:
                            if (IsShielded) break;
                            CameraShakeController.Instance.DoShake(0);
                            StreakController.Instance.Clear();
                            GameController.Instance.EndGame();
                            break;

                        case CollectableType.Cofee:
                            TempEffectsController.Instance.Activate(CollectableType.Cofee);
                            Blackboard.OnBuffPickUp.Execute();
                            break;

                        case CollectableType.Energy:
                            TempEffectsController.Instance.Activate(CollectableType.Energy);
                            Blackboard.OnBuffPickUp.Execute();
                            break;

                        case CollectableType.Shield:
                            TempEffectsController.Instance.Activate(CollectableType.Shield);
                            Blackboard.OnBuffPickUp.Execute();
                            break;

                        case CollectableType.ControlVirus:
                            if (IsShielded) break;
                            TempEffectsController.Instance.Activate(CollectableType.ControlVirus);
                            OnNegativeEffect(collectable);
                            break;

                        case CollectableType.TransitionSpeedVirus:
                            if (IsShielded) break;
                            TempEffectsController.Instance.Activate(CollectableType.TransitionSpeedVirus);
                            OnNegativeEffect(collectable);
                            break;

                        case CollectableType.MergeLockVirus:
                            if (IsShielded) break;
                            TempEffectsController.Instance.Activate(CollectableType.MergeLockVirus);
                            OnNegativeEffect(collectable);
                            break;
                    }

                    collectable.Collect();
                }
            }

            void OnNegativeEffect(Collectable collectable)
            {
                Blackboard.OnBadPickUp.Execute();

                CameraShakeController.Instance.DoShake(0);
                StreakController.Instance.Clear();

                var codeLossAmount = Mathf.RoundToInt(Blackboard.CodeCollected.Value * Blackboard.CodeLossModifier.Value);

                if (codeLossAmount == 0 && Blackboard.CodeCollected.Value > 0)
                {
                    codeLossAmount = Blackboard.CodeCollected.Value;
                    Blackboard.CodeCollected.Value = 0;
                }
                else
                {
                    Blackboard.CodeCollected.Value -= codeLossAmount;
                }

                if (codeLossAmount > 0)
                {
                    var text = _textPool.Take().GetComponent<CollectableText>();
                    text.transform.position = collectable.transform.position;// + Vector3.up * .5f;
                    text.Animate("-" + codeLossAmount.ToString(), 1f, BranchesColorIndex.Red);
                }
            }
        }

        private enum TransitionDirection
        {
            None, Left, Right
        }
    }
}
