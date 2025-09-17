using Az7.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;

namespace Branches
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
        }

        private async UniTaskVoid StartGameAsync(CancellationToken token)
        {
            _gameStart.Play();

            UI_ControllerSingle.Instance.HideView(UI_ViewKey.Main);

            _music.Play();

            await UniTask.WhenAll(
                UI_ControllerSingle.Instance.ShowViewAsync(UI_ViewKey.Game, .2f, token),
                GameSpeedController.Instance.SetRatioAsync(1f, .5f, token),
                MixingCameraController.Instance.ToggleCameraAsync(true, .5f, token)
                );

            CollectablesSpawner.Instance.StartSpawning();
            Blackboard.GameState.Value = GameState.Running;

            PlayerController.Instance.MoveRight(true);
            PlayerController.Instance.MoveRight(false);
        }

        private async UniTaskVoid PrepareNewGameAsync(CancellationToken token)
        {
            Blackboard.GameState.Value = GameState.None;

            CurtainSingle.Instance.ShowImmidiate();

            UI_ControllerSingle.Instance.HideAll();
            UI_ControllerSingle.Instance.ShowView(UI_ViewKey.Main);

            PlayerController.Instance.Setup();
            GameSpeedController.Instance.Setup();
            MixingCameraController.Instance.ToggleCameraImmidiate(false);
            TempEffectsController.Instance.ClearAll();
            StreakController.Instance.Clear();
            PerkController.Instance.Clear();
            CollectablesSpawner.Instance.StopAndClear();
            BranchController.Instance.DeactivateAll();

            //core
            Blackboard.Version.Value = startLevel;
            Blackboard.MaxLanesCount.Value = 3;

            //code
            Blackboard.BugsCollected.Value = 0;
            Blackboard.CodeCollected.Value = 0;
            Blackboard.CodeStored.Value = 0;
            //required is calculated

            //stats
            Blackboard.TransitionSpeed.Value = 1f;
            Blackboard.TransitionSpeedModifier.Value = 1f;
            Blackboard.PerkTransitionSpeedModifier.Value = 1f;
            Blackboard.MergeEventTimeModifier.Value = 1f;
            Blackboard.BuffTimeModifier.Value = 1f;
            Blackboard.DebuffTimeModifier.Value = 1f;
            Blackboard.CodeLossModifier.Value = .5f;
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


        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.F5))
        //    {
        //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //    }

        //    if (Input.GetKeyDown(KeyCode.F6))
        //    {
        //        GameSpeedController.Instance.SetSpeed(Blackboard.MovementSpeed.Value - 1f);
        //    }

        //    if (Input.GetKeyDown(KeyCode.F7))
        //    {
        //        GameSpeedController.Instance.SetSpeed(Blackboard.MovementSpeed.Value + 1f);
        //    }
        //}


    }
}
