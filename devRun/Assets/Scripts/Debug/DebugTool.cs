using UnityEngine;

namespace DevRun
{
    public class DebugTool : MonoBehaviour
    {
        [SerializeField] private float _gameSpeed;

        private void OnValidate()
        {
            if (GameSpeedController.Instance == null) return;

            GameSpeedController.Instance.SetSpeed(_gameSpeed);
        }
    }
}
