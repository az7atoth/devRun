using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using Timer = Az7.Utils.Timers.Timer;

namespace Branches
{
    public class MixingCameraController : MonoBehaviour
    {
        public static MixingCameraController Instance { get; private set; }

        [SerializeField] private CinemachineMixingCamera _mixingCamera;

        private Timer _timer = new Timer();

        public void ToggleCameraImmidiate(bool gameCamera)
        {
            if (gameCamera)
            {
                _mixingCamera.Weight0 = 1.0f;
                _mixingCamera.Weight1 = 0.0f;
            }
            else
            {
                _mixingCamera.Weight0 = 0.0f;
                _mixingCamera.Weight1 = 1.0f;
            }
        }

        public async UniTask ToggleCameraAsync(bool gameCamera, float transitionTime, CancellationToken token)
        {
            _timer.Set(transitionTime);
            _timer.Start();

            while (_timer.State != Az7.Utils.Timers.TimerState.Completed)
            {
                var startValueW0 = _mixingCamera.Weight0;
                var startValueW1 = _mixingCamera.Weight1;

                _mixingCamera.Weight0 = Mathf.Lerp(startValueW0, gameCamera ? 1f : 0f, _timer.Ratio);
                _mixingCamera.Weight1 = Mathf.Lerp(startValueW1, gameCamera ? 0f : 1f, _timer.Ratio);

                _timer.Update(Time.deltaTime);

                await UniTask.Yield();

                if (token.IsCancellationRequested)
                {
                    _timer.Stop();
                    return;
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
        }

    }
}
