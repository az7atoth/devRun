using Az7.Extensions;
using Az7.Utils.Pool;
using DevRun;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Collections.Generic;

public class PatternBasedSpawner : MonoBehaviour, ICollectablesSpawner
{
    private const int LANES_COUNT = 3;

    private const char CODE_CHAR = 'C';
    private const char BUG_CHAR = 'B';
    private const char FIX_CHAR = 'F';
    private const char BUFF_CHAR = 'P';
    private const char DEBUFF_CHAR = 'D';
    private const char EMPTY_CHAR = '.';

    [SerializeField] private AnimationCurve _spawnIntervalCurve;
    [SerializeField] private Pool _pool;

    private SpawnPatternProvider _patternProvider;

    private bool _isSpawning;
    private float _spawnTimer;
    private int _currentRowCounter;
    private int _currentPatternRowsCount;
    private string _currentPattern;
    private List<Collectable> _activeCollectables = new(30);
    private List<Collectable> _inactiveCollectables = new(10);

    [Inject]
    public void Construct(SpawnPatternProvider patternProvider)
    {
        _patternProvider = patternProvider;
    }

    public void StartSpawning()
    {
        _patternProvider.DefineGamePhase(Blackboard.Level.Value);
        _isSpawning = true;
        _spawnTimer = 0f;
        _currentRowCounter = 0;
    }

    public void StopAndClear()
    {
        _isSpawning = false;

        foreach (var collectable in _activeCollectables)
        {
            collectable.Deactivate(false);
        }

        _activeCollectables.Clear();
    }

    public void ShowAll(float transitionTime)
    {
        foreach (var collectable in _activeCollectables)
        {
            collectable.Show(transitionTime);
        }
    }

    public void HideAll(float transitionTime)
    {
        foreach (var collectable in _activeCollectables)
        {
            collectable.Hide(transitionTime);
        }
    }

    private void Awake()
    {
        Collectable.OnDeactivate.Subscribe(collectable =>
        {
            _activeCollectables.Remove(collectable);
        }).AddTo(this);
    }

    private void FixedUpdate()
    {
        if (!_isSpawning) return;

        if (_spawnTimer > 0f)
        {
            _spawnTimer -= Time.fixedDeltaTime * Blackboard.GameSpeedRatio.Value
            * GameSpeedController.Instance.MovementSpeedRatio * Blackboard.GameSpeedRatioModifier.Value;
        }

        if (_spawnTimer <= 0f)
        {
            Spawn();
            _spawnTimer = GetSpawnInterval();
        }

        //check and move collectables
        if (_inactiveCollectables.Count > 0)
        {
            foreach (var collectable in _inactiveCollectables)
            {
                _activeCollectables.Remove(collectable);
                collectable.Deactivate(false);
            }
        }

        _inactiveCollectables.Clear();

        foreach (var collectable in _activeCollectables)
        {
            collectable.transform.position += Vector3.down
                * Time.fixedDeltaTime
                * Blackboard.MovementSpeed.Value
                * Blackboard.GameSpeedRatio.Value
                * Blackboard.GameSpeedRatioModifier.Value;

            if (collectable.transform.position.y + .5f < PlayerController.Instance.transform.position.y)
            {
                collectable.HideByDistance();

                //if (collectable.Type == CollectableType.Code && collectable.IsActive)
                //{
                //    collectable.IsActive = false;
                //    StreakController.Instance.Clear();
                //}
            }

            if (collectable.transform.position.y < PositionProvider.Instance.ScreenBottom.position.y)
            {
                _inactiveCollectables.Add(collectable);
            }
        }
    }

    private void Spawn()
    {
        if (_currentRowCounter == 0)
        {
            _currentPattern = _patternProvider.GetPattern(LANES_COUNT); //TODO lanes count
            _currentPatternRowsCount = _currentPattern.Length / LANES_COUNT;
            //ebug.Log($"Current pattern changed to: {_currentPattern}");
        }

        //Debug.Log($"Row: {_currentRowCounter}");

        var targetCharNumber = 0;

        for (int i = 0; i < LANES_COUNT; i++)
        {
            var collectableType = CollectableType.None;

            targetCharNumber = LANES_COUNT * _currentRowCounter + i;

            if (targetCharNumber < _currentPattern.Length)
            {
                var targetChar = _currentPattern[targetCharNumber];
                collectableType = DefineCollectableType(targetChar);
                //Debug.Log($"Spawn {collectableType} from {targetChar}; num: {targetCharNumber}");
            }
            else
            {
                //Debug.Log($"Spawn empty");
            }

            if (collectableType == CollectableType.None) continue;

            var spawnPositionX = PositionProvider.Instance.LaneStep * (i + 1);
            var spawnPosition = new Vector3(spawnPositionX, PositionProvider.Instance.ScreenTop.position.y, 0f);

            var collectable = _pool.Take().GetComponent<Collectable>();
            collectable.transform.position = spawnPosition;
            _activeCollectables.Add(collectable);

            collectable.Activate(collectableType);
        }

        if (targetCharNumber + 1 >= _currentPattern.Length)
        {
            _currentRowCounter = 0;
        }
        else
        {
            _currentRowCounter++;
        }
    }

    private CollectableType DefineCollectableType(char c)
    {
        if (char.Equals(c, CODE_CHAR))
        {
            return CollectableType.Code;
        }
        else
        if (char.Equals(c, FIX_CHAR))
        {
            return CollectableType.Fix;
        }
        else
        if (char.Equals(c, BUG_CHAR))
        {
            return CollectableType.Bug;
        }
        else
        if (char.Equals(c, BUFF_CHAR))
        {
            return CollectableType.None; //TODO temp
        }
        else
        if (char.Equals(c, DEBUFF_CHAR))
        {
            return CollectableType.None; //TODO temp
        }
        else
        {
            return CollectableType.None;
        }
    }

    private float GetSpawnInterval()
    {
        return _spawnIntervalCurve.Evaluate(Blackboard.Difficulty.Value);
    }
}
