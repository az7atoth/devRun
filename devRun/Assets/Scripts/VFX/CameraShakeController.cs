using Az7.Utils.Timers;
using Unity.Cinemachine;
using UnityEngine;

namespace Branches
{
    public class CameraShakeController : MonoBehaviour
    {
        public static CameraShakeController Instance { get; private set; }

        [SerializeField] private CinemachineBasicMultiChannelPerlin[] _cameras;
        [SerializeField] float _amplitude = 5f;
        [SerializeField] float _frequency = 1f;
        [SerializeField] float _defaultDuration = .2f;

        private Timer[] _timers;

        public void DoShake(int cameraIndex, float duration = 0f)
        {
            if (duration <= 0f)
            {
                duration = _defaultDuration;
            }

            _cameras[cameraIndex].AmplitudeGain = _amplitude;
            _cameras[cameraIndex].FrequencyGain = _frequency;
            _timers[cameraIndex].Set(duration);
            _timers[cameraIndex].Start();
        }

        private void Update()
        {
            for (int i = 0; i < _timers.Length; i++)
            {
                if (_timers[i].State == TimerState.Stopped)
                {
                    continue;
                }
                else if (_timers[i].State == TimerState.Completed)
                {
                    _timers[i].Stop();
                    _cameras[i].AmplitudeGain = 0f;
                    _cameras[i].FrequencyGain = 0f;
                }
                else
                {
                    _timers[i].Update(Time.deltaTime);
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
                return;
            }

            _timers = new Timer[_cameras.Length];

            for (int i = 0; i < _timers.Length; i++)
            {
                _timers[i] = new Timer();
            }
        }
    }
}
