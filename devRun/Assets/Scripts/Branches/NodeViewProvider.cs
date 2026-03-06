using Az7.Utils.Pool;
using UnityEngine;

namespace DevRun
{
    public class NodeViewProvider : MonoBehaviour
    {
        public static NodeViewProvider Instance { get; private set; }

        [SerializeField] private Pool _nodeViewPool;

        public NodeView Get()
        {
            return _nodeViewPool.Take().GetComponent<NodeView>();
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
    }
}
