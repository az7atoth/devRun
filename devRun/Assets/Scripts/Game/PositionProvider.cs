using UnityEngine;

namespace Branches
{
    public class PositionProvider : MonoBehaviour
    {
        public static PositionProvider Instance { get; private set; }

        [field: SerializeField] public Transform ScreenTop { get; private set; }
        [field: SerializeField] public Transform ScreenBottom { get; private set; }
        [field: SerializeField] public Transform MainBranchStart { get; private set; }
        [field: SerializeField] public float LaneStep { get; private set; }

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
