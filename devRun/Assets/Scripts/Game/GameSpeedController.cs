using Az7.Utils;
using DevRun;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UniRx;

public class GameSpeedController : MonoBehaviour
{
    public static GameSpeedController Instance { get; private set; }

    public float MovementSpeedRatio => Blackboard.MovementSpeed.Value / _minSpeed;

    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private float _defaultTransitionTime = .3f;

    private CancellationTokenSource _ratioCts;
    private CancellationTokenSource _ratioModCts;

    public void Setup()
    {
        Blackboard.MovementSpeed.Value = _minSpeed;
        Blackboard.GameSpeedRatio.Value = 1f;
        Blackboard.GameSpeedRatioModifier.Value = 1f;
    }

    public void SetSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, _minSpeed, _maxSpeed);
        Blackboard.MovementSpeed.Value = speed;
    }

    public void SetRatio(float ratio, float transitionTime = 0f)
    {
        CancelRatio();
        _ratioCts = new();
        SetRatioAsync(ratio, transitionTime, _ratioCts.Token).Forget();
    }

    public void SetRatioModifier(float ratio, float transitionTime = 0f)
    {
        CancelRatioMod();
        _ratioModCts = new();
        SetRatioModAsync(ratio, transitionTime, _ratioModCts.Token).Forget();
    }

    public async UniTask SetRatioAsync(float ratio, float transitionTime, CancellationToken token)
    {
        ratio = Mathf.Clamp01(ratio);

        if (transitionTime == 0f)
        {
            transitionTime = _defaultTransitionTime;
        }

        var startValue = Blackboard.GameSpeedRatio.Value;

        var t = 0f;
        var time = 0f;

        while (t < 1f)
        {
            t = time / transitionTime;
            t = EaseFunctions.EaseInOutCubic(t);
            Blackboard.GameSpeedRatio.Value = Mathf.Lerp(startValue, ratio, t);

            await UniTask.Yield();
            if (token.IsCancellationRequested) return;

            time += Time.deltaTime;
        }

        Blackboard.GameSpeedRatio.Value = ratio;
    }

    public async UniTask SetRatioModAsync(float ratio, float transitionTime, CancellationToken token)
    {
        if (transitionTime == 0f)
        {
            transitionTime = _defaultTransitionTime;
        }

        var startValue = Blackboard.GameSpeedRatioModifier.Value;

        var t = 0f;
        var time = 0f;

        while (t < 1f)
        {
            t = time / transitionTime;
            t = EaseFunctions.EaseInOutCubic(t);
            Blackboard.GameSpeedRatioModifier.Value = Mathf.Lerp(startValue, ratio, t);

            await UniTask.Yield();
            if (token.IsCancellationRequested) return;

            time += Time.deltaTime;
        }

        Blackboard.GameSpeedRatioModifier.Value = ratio;
    }

    private void CancelRatio()
    {
        _ratioCts?.Cancel();
        _ratioCts?.Dispose();
        _ratioCts = null;
    }

    private void CancelRatioMod()
    {
        _ratioModCts?.Cancel();
        _ratioModCts?.Dispose();
        _ratioModCts = null;
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

        Blackboard.Difficulty.Subscribe(value =>
        {
            Blackboard.MovementSpeed.Value = _speedCurve.Evaluate(value);
        }).AddTo(this);
    }

    private void OnDestroy()
    {
        CancelRatio();
        CancelRatioMod();
    }
}
