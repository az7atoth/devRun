using Az7.Utils.Pool;
using UnityEngine;

namespace Branches
{
    public class Branch : MonoBehaviour
    {
        public Vector3 StartPosition { get => _lineRenderer.GetPosition(0); set => _lineRenderer.SetPosition(0, value); }
        public Vector3 KneePosition { get => _lineRenderer.GetPosition(1); set => _lineRenderer.SetPosition(1, value); }
        public Vector3 EndPosition { get => _lineRenderer.GetPosition(2); set => _lineRenderer.SetPosition(2, value); }
        public bool IsMoving { get; set; }


        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private PoolableObject _poolableObject;
        [SerializeField] private float _afterLeaveDistance = 2f;
        [SerializeField] private float _endNodeSpeedModifier = .2f;

        private NodeView _startNode;
        private NodeView _endNode;
        private bool _isEnded;
        private bool _isAbandoned;
        private bool _kneeIsSetted;
        private Vector3 _leavePosition;

        public void Activate(Vector3 startPosition)
        {
            //self
            _lineRenderer.SetPosition(0, startPosition);
            _lineRenderer.SetPosition(1, startPosition);
            _lineRenderer.SetPosition(2, startPosition);

            gameObject.SetActive(true);
            _isEnded = false;
            _isAbandoned = false;
            _kneeIsSetted = false;
            IsMoving = true;

            //start node
            _startNode = NodeViewProvider.Instance.Get();
            _startNode.transform.position = startPosition;
            _startNode.gameObject.SetActive(true);
        }

        public void SetStart(Vector3 position)
        {
            _lineRenderer.SetPosition(0, position);
            _startNode.transform.position = position;

            if (!_kneeIsSetted)
            {
                _lineRenderer.SetPosition(1, position);
            }
        }

        public void SetKnee(Vector3 position)
        {
            _lineRenderer.SetPosition(1, position);
            _kneeIsSetted = true;
        }

        public void SetEnd(Vector3 position)
        {
            _lineRenderer.SetPosition(2, position);
        }

        public void Leave(Vector3 position)
        {
            _endNode = NodeViewProvider.Instance.Get();
            _endNode.transform.position = position;
            _endNode.Activate();

            _leavePosition = position;
            _isAbandoned = true;
        }

        public void LeaveImmidiate(Vector3 position)
        {
            _endNode = NodeViewProvider.Instance.Get();
            _endNode.transform.position = position;
            _endNode.Activate();

            _leavePosition = position;
            //_isAbandoned = true;
            _isEnded = true;
        }

        public void Move(Vector3 delta)
        {
            StartPosition += delta;
            KneePosition += delta;

            _startNode.transform.position += delta;

            if (_isAbandoned)
            {
                _leavePosition += delta;

                if (!_isEnded && Vector3.Distance(_leavePosition, EndPosition)
                    >= (_afterLeaveDistance / (Blackboard.TransitionSpeed.Value * Blackboard.TransitionSpeedModifier.Value * Blackboard.PerkTransitionSpeedModifier.Value)))
                {
                    _isEnded = true;
                }
            }

            if (_endNode != null)
            {
                if (_isEnded)
                {
                    EndPosition += delta;
                    _endNode.transform.position += delta;
                }
                else
                {
                    EndPosition -= delta * _endNodeSpeedModifier;
                    _endNode.transform.position -= delta * _endNodeSpeedModifier;
                }
            }
        }

        public void Deactivate()
        {
            _startNode?.Deactivate();
            _endNode?.Deactivate();

            _startNode = null;
            _endNode = null;

            if (_poolableObject.ReturnRequested) return;

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

        private void Awake()
        {
            _lineRenderer.positionCount = 3;
        }


    }
}
