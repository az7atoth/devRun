using Az7.UI;
using Az7.UserInput;
using Az7.Utils.Timers;
using UnityEngine;
using UniRx;
using Az7.Utils.Disposables;
using Az7.Extensions;
using TMPro;
using System;
using Random = UnityEngine.Random;
using Az7.Utils.Pool;
using UnityEngine.UI;

namespace DevRun
{
    public class MergeUI : UI_ViewBase
    {
        public override UI_ViewKey ViewKey => UI_ViewKey.Merge;

        [SerializeField] private AnimationCurve _timeCurve;
        [SerializeField] private Image _timerFill;
        [SerializeField] private TMP_Text _iterationsCounter;
        [SerializeField] private MergeCodeBlock[] _left;
        [SerializeField] private MergeCodeBlock[] _right;
        [SerializeField] private MergeSelector[] _selectors;
        [SerializeField] private BranchesColorIndex _goodColor = BranchesColorIndex.Green;
        [SerializeField] private BranchesColorIndex _badColor = BranchesColorIndex.Red;
        [SerializeField] private BranchesColorIndex _neutralColor = BranchesColorIndex.Black;
        [SerializeField] private Pool _fxPool;

        public bool EventOvered { get; private set; } = true;
        public bool EventResult { get; private set; }

        private Timer _timer = new();
        private DisposableTracker _disposableTracker = new();
        private float _eventDuration;
        private MergeAction[] _puzzle;
        private bool _puzzleIsSettedUp;
        private int _partsSolved;
        private int _iterationsRequired;
        private int _iterations;


        public void Activate()
        {
            InputProviderSingleMono.Instance.OnMoveLeft.Subscribe(pressed => { PlayerAction(pressed, MergeAction.Left); }).AddTo(_disposableTracker);
            InputProviderSingleMono.Instance.OnMoveRight.Subscribe(pressed => { PlayerAction(pressed, MergeAction.Right); }).AddTo(_disposableTracker);
            InputProviderSingleMono.Instance.OnAction.Subscribe(pressed => { PlayerAction(pressed, MergeAction.Skip); }).AddTo(_disposableTracker);
        }

        public void Prepare()
        {
            EventOvered = false;
            EventResult = false;

            _eventDuration = _timeCurve.Evaluate(Blackboard.Difficulty.Value);
            _eventDuration *= Blackboard.MergeEventTimeModifier.Value;

            _timer.Set(_eventDuration);
            _timer.Start();

            SetupPuzzle();
            ShowSelector(0, true);
            _selectors[0].SetIcon(MergeSelector.SelectorState.Question);
            _puzzleIsSettedUp = true;

            _iterationsRequired = Blackboard.MergeEventIterations.Value;
            _iterations = 1;

            SetIterationsCounterText();
        }

        private void PlayerAction(bool pressed, MergeAction action)
        {
            if (!_puzzleIsSettedUp || !pressed || EventOvered) return;

            var fx = _fxPool.Take().GetComponent<ParticleSystem>();
            var main = fx.main;

            if (_puzzle[_partsSolved] == action)
            {
                _left[_partsSolved].Select();
                _right[_partsSolved].Select();

                main.startColor = ColorProvider.Instance.Get(_goodColor);

                var selectorIconIndex = (int)action + 1;
                _selectors[_partsSolved].SetIcon((MergeSelector.SelectorState)selectorIconIndex);

                _partsSolved++;
                Blackboard.OnMergeCorrect.Execute();

                if (_partsSolved < _selectors.Length)
                {
                    ShowSelector(_partsSolved, false);
                    _selectors[_partsSolved].SetIcon(MergeSelector.SelectorState.Question);
                }

                if (_partsSolved == _puzzle.Length)
                {
                    _iterations++;

                    if (_iterations > _iterationsRequired)
                    {
                        EventResult = true;
                        EventOvered = true;
                    }
                    else
                    {
                        SetIterationsCounterText();
                        SetupPuzzle();
                        ShowSelector(0, true);
                        _selectors[0].SetIcon(MergeSelector.SelectorState.Question);
                    }
                }
            }
            else
            {
                Blackboard.OnBadPickUp.Execute();

                main.startColor = ColorProvider.Instance.Get(_badColor);

                CameraShakeController.Instance.DoShake(1);

                SetupPuzzle();
            }

            fx.gameObject.SetActive(true);
            fx.Play();
        }

        private void SetupPuzzle()
        {
            for (int i = 0; i < _puzzle.Length; i++)
            {
                var action = (MergeAction)Random.Range(0, 3);

                if (i > 0)
                {
                    while (_puzzle[i - 1] == action)
                    {
                        action = (MergeAction)Random.Range(0, 3);
                    }
                }

                _puzzle[i] = action;

                switch (_puzzle[i])
                {
                    case MergeAction.Left:
                        _left[i].Activate(_goodColor);
                        _right[i].Activate(_badColor);
                        break;

                    case MergeAction.Right:
                        _left[i].Activate(_badColor);
                        _right[i].Activate(_goodColor);
                        break;

                    case MergeAction.Skip:
                        _left[i].Activate(_neutralColor);
                        _right[i].Activate(_neutralColor);
                        break;
                }
            }

            _partsSolved = 0;
        }

        private void ShowSelector(int index, bool hideOthers)
        {
            for (int i = 0; i < _selectors.Length; i++)
            {
                if (i == index)
                {
                    _selectors[i].Show();
                }
                else
                {
                    if (hideOthers)
                    {
                        _selectors[i].Hide();
                    }
                }
            }
        }

        private void Update()
        {
            if (EventOvered) { return; }

            if (_timer.State == TimerState.Completed)
            {
                EventOvered = true;
            }

            _timer.Update(Time.deltaTime);
            _timerFill.fillAmount = 1f - _timer.Ratio;
        }

        public void Deactivate()
        {
            _disposableTracker.Dispose();
        }

        private void SetIterationsCounterText()
        {
            _iterationsCounter.text = _iterations + "/" + _iterationsRequired;
        }

        private void Awake()
        {
            _puzzle = new MergeAction[_left.Length];
        }

        private enum MergeAction
        {
            Left = 0,
            Right = 1,
            Skip = 2,
        }
    }
}
