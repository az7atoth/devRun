using Az7.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using Zenject;

namespace DevRun
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [SerializeField] private bool _initializeOnStart = true;
        [SerializeField] private int startLevel = 3;
        [SerializeField] private GameObject _endGameImage;
        [SerializeField] private PlayableDirector _endGameDirector;
        [SerializeField] private AudioSource _music;
        [SerializeField] private AudioSource _gameStart;

        private UI_Controller _uI_Controller;
        private ICollectablesSpawner _collectablesSpawner;
        private IScoreCounter _scoreCounter;

        private float _startTime;

        [Inject]
        public void Construct(
            UI_Controller uI_Controller,
            ICollectablesSpawner spawner,
            IScoreCounter scoreCounter
            )
        {
            _uI_Controller = uI_Controller;
            _collectablesSpawner = spawner;
            _scoreCounter = scoreCounter;
        }

        public void Initialize()
        {
            PlayerController.Instance.Initialize();
            PrepareNewGameAsync(destroyCancellationToken).Forget();
        }

        public void StartNewGame()
        {
            if (Blackboard.GameState.Value != GameState.Init) return;
            StartGameAsync(destroyCancellationToken).Forget();
        }

        public void EndGame()
        {
            EndGameAsync(destroyCancellationToken).Forget();
            _music.Stop();

            var sessionTime = Time.unscaledTime - _startTime;
            Debug.Log($"Session time: {sessionTime}; Actions: {Blackboard.ActionsCounter}");
        }

        private async UniTaskVoid StartGameAsync(CancellationToken token)
        {
            _gameStart.Play(); //play start sfx

            _uI_Controller.HideViewImmidiate(UI_ViewKey.Main);

            _music.Play(); //play music

            await UniTask.WhenAll(
                _uI_Controller.ShowViewAsync(UI_ViewKey.Game, .2f, token),
                GameSpeedController.Instance.SetRatioAsync(1f, .5f, token),
                MixingCameraController.Instance.ToggleCameraAsync(true, .5f, token)
                );

            _collectablesSpawner.StartSpawning();
            Blackboard.GameState.Value = GameState.Running;

            PlayerController.Instance.MoveRight(true);
            PlayerController.Instance.MoveRight(false);

            _startTime = Time.unscaledTime;
            Blackboard.ActionsCounter = 0;
        }

        private async UniTaskVoid PrepareNewGameAsync(CancellationToken token)
        {
            Blackboard.GameState.Value = GameState.None;

            CurtainSingle.Instance.ShowImmidiate();

            _uI_Controller.HideAll();
            _uI_Controller.ShowViewImmidiate(UI_ViewKey.Main);

            PlayerController.Instance.Setup();
            GameSpeedController.Instance.Setup();
            MixingCameraController.Instance.ToggleCameraImmidiate(false);
            TempEffectsController.Instance.ClearAll();
            PerkController.Instance.Clear();
            _collectablesSpawner.StopAndClear();
            BranchController.Instance.DeactivateAll();

            //core
            Blackboard.Level.Value = startLevel;
            Blackboard.MaxLanesCount.Value = 3;

            //code
            Blackboard.BugsCollected.Value = 0;
            _scoreCounter.ClearCollected();
            _scoreCounter.ClearStored();
            _scoreCounter.UpdateRequestedScore(Blackboard.Level.Value);

            //stats
            Blackboard.TransitionSpeed.Value = 1f;
            Blackboard.TransitionSpeedModifier.Value = 1f;
            Blackboard.PerkTransitionSpeedModifier.Value = 1f;
            Blackboard.MergeEventTimeModifier.Value = 1f;
            Blackboard.BuffTimeModifier.Value = 1f;
            Blackboard.DebuffTimeModifier.Value = 1f;
            Blackboard.CodeLossModifier.Value = .12f;
            Blackboard.BugLimit.Value = 3;

            Blackboard.GameSpeedRatio.Value = 0f;

            await CurtainSingle.Instance.HideAsync(token);

            Blackboard.GameState.Value = GameState.Init;
        }

        private async UniTaskVoid EndGameAsync(CancellationToken token)
        {
            Blackboard.GameState.Value = GameState.Overed;
            Blackboard.GameSpeedRatio.Value = 0f;

            //_music.Stop();

            _endGameDirector.Play();

            await UniTask.WaitUntil(() => _endGameDirector.state == PlayState.Paused, cancellationToken: token)
                .SuppressCancellationThrow();

            if (token.IsCancellationRequested) return;

            PrepareNewGameAsync(token).Forget();
        }

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (_initializeOnStart)
            {
                Initialize();
            }

            Blackboard.BugsCollected.Subscribe(value =>
            {
                if (Blackboard.GameState.Value != GameState.Running)
                {
                    return;
                }

                if (value > Blackboard.BugLimit.Value)
                {
                    EndGame();
                }
            }).AddTo(this);
        }

    }
}
