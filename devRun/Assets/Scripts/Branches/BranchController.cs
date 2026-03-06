using Az7.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace DevRun
{
    public class BranchController : MonoBehaviour
    {
        public static BranchController Instance { get; private set; }

        [SerializeField] private Pool _branchesPool;
        [SerializeField] private bool _initializeOnAwake = true;

        private bool _isInitialized;
        private List<Branch> _activeBranches;
        private List<Branch> _inactiveBranches;

        public void Initialize()
        {
            if (_isInitialized) return;

            _activeBranches = new List<Branch>(30);
            _inactiveBranches = new List<Branch>(10);

            _isInitialized = true;
        }

        public Branch Get()
        {
            var branch = _branchesPool.Take().GetComponent<Branch>();
            _activeBranches.Add(branch);
            return branch;
        }

        public void DeactivateAll()
        {
            foreach (var branch in _activeBranches)
            {
                branch.Deactivate();
            }

            _activeBranches.Clear();
        }

        private void Update()
        {
            if (Blackboard.GameState.Value != GameState.Running) return;

            var movementDelta = Vector3.down * Time.deltaTime
                * Blackboard.MovementSpeed.Value * Blackboard.GameSpeedRatio.Value * Blackboard.GameSpeedRatioModifier.Value;

            if (_inactiveBranches.Count > 0)
            {
                foreach (var branch in _inactiveBranches)
                {
                    branch.Deactivate();
                    _activeBranches.Remove(branch);
                }

                _inactiveBranches.Clear();
            }

            foreach (var branch in _activeBranches)
            {
                if (!branch.IsMoving) continue;

                branch.Move(movementDelta);

                if (branch.EndPosition.y < PositionProvider.Instance.ScreenBottom.position.y)
                {
                    _inactiveBranches.Add(branch);
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

            if (_initializeOnAwake)
            {
                Initialize();
            }
        }

    }
}
