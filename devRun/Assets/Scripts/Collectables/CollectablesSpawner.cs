using Az7.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Branches
{
    public class CollectablesSpawner : MonoBehaviour
    {
        public static CollectablesSpawner Instance { get; private set; }

        [SerializeField] private Pool _pool;

        [SerializeField] private AnimationCurve _minSpawnDistanceCurve;
        [SerializeField] private AnimationCurve _maxSpawnDistanceCurve;
        [SerializeField] private AnimationCurve _negativeAmount;
        [SerializeField] private AnimationCurve _negativeSpecialsAmount;
        [SerializeField] private AnimationCurve _positiveSpecialsAmount;

        [SerializeField] private float _startSpawnDelay = 2f;

        private List<Collectable> _activeCollectables = new(10);
        private List<Collectable> _inactiveCollectables = new(10);
        private Dictionary<int, int> _laneStats = new Dictionary<int, int>(5);
        private List<int> _possibleLanes = new(5);
        private Collectable _lastSpawned;
        private bool _isSpawning;
        private float _spawnTimer;
        private int _lastLaneIndex;
        private float _spawnDistance;

        public void StartSpawning()
        {
            //if (_timers.Count != Blackboard.MaxLanesCount.Value)
            //{
            //    _timers.Clear();

            //    for (int i = 0; i < Blackboard.MaxLanesCount.Value; i++)
            //    {
            //        _timers.Add(new Timer(_startSpawnDelay + GetRandomSpawnInterval()));
            //    }
            //}
            //else
            //{
            //    foreach (var timer in _timers)
            //    {
            //        timer.Set(_startSpawnDelay + GetRandomSpawnInterval());
            //    }
            //}

            //foreach (var timer in _timers)
            //{
            //    timer.Start();
            //}

            _laneStats.Clear();
            _isSpawning = true;
            _spawnTimer = _startSpawnDelay;
            _spawnDistance = GetRandomSpawnDistance();
            _lastLaneIndex = -1;
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

        public void HideAll(float transitionTime)
        {
            foreach (var collectable in _activeCollectables)
            {
                collectable.Hide(transitionTime);
            }
        }

        public void ShowAll(float transitionTime)
        {
            foreach (var collectable in _activeCollectables)
            {
                collectable.Show(transitionTime);
            }
        }

        private void Spawn()
        {
            var spawnIndex = GetLane();

            var spawnPositionX = PositionProvider.Instance.LaneStep * spawnIndex;
            var spawnPosition = new Vector3(spawnPositionX, PositionProvider.Instance.ScreenTop.position.y, 0f);

            var collectable = _pool.Take().GetComponent<Collectable>();
            collectable.transform.position = spawnPosition;
            _activeCollectables.Add(collectable);
            _lastSpawned = collectable;

            var collectableType = GetCollectableType();

            collectable.Activate(collectableType);
        }

        private int GetLane()
        {
            var result = 0;
            _possibleLanes.Clear();

            if (_laneStats.Count < Blackboard.MaxLanesCount.Value)
            {
                _laneStats.Clear();

                while (_laneStats.Count != Blackboard.MaxLanesCount.Value)
                {
                    _laneStats.Add(_laneStats.Count, 0);
                }
            }

            var maxCount = 0;

            foreach (var lane in _laneStats)
            {
                if (lane.Value > maxCount)
                {
                    maxCount = lane.Value;
                }
            }

            foreach (var lane in _laneStats)
            {
                if (lane.Value < maxCount)
                {
                    _possibleLanes.Add(lane.Key);
                }
            }

            if (_possibleLanes.Count == 0)
            {
                do
                {
                    result = Random.Range(0, Blackboard.MaxLanesCount.Value);
                } while (result == _lastLaneIndex);
            }
            else
            {
                var rndIndex = Random.Range(0, _possibleLanes.Count);
                result = _possibleLanes[rndIndex];
            }

            //var str = "Lane Stats: ";

            //foreach (var lane in _laneStats)
            //{
            //    str += lane.Value + "; ";
            //}

            //Debug.Log(str + "possible: " + _possibleLanes.Count + "; result: " + result);

            _lastLaneIndex = result;
            _laneStats[result]++;

            return result + 1;
        }

        private CollectableType GetCollectableType()
        {
            CollectableType result;
            var rndVal = Random.value;

            var isNegative = rndVal < _negativeAmount.Evaluate(Blackboard.Difficulty.Value);

            if (isNegative)
            {
                rndVal = Random.value;

                var isSpecial = rndVal < _negativeSpecialsAmount.Evaluate(Blackboard.Difficulty.Value);

                //Debug.Log("Is negative, special: " + isSpecial);

                if (isSpecial)
                {
                    rndVal = Random.value;

                    if (rndVal <= .3f)
                    {
                        result = CollectableType.ControlVirus;
                    }
                    else if (rndVal <= .6f)
                    {
                        result = CollectableType.TransitionSpeedVirus;
                    }
                    else if (rndVal <= .9f)
                    {
                        result = CollectableType.MergeLockVirus;
                    }
                    else
                    {
                        if (Blackboard.Difficulty.Value < .3f)
                        {
                            result = CollectableType.Bug;
                        }
                        else
                        {
                            result = CollectableType.Death;
                        }
                    }
                }
                else
                {
                    result = CollectableType.Bug;
                }
            }
            else
            {
                rndVal = Random.value;

                var isSpecial = rndVal < _positiveSpecialsAmount.Evaluate(Blackboard.Difficulty.Value);

                //Debug.Log("Is positive, special: " + isSpecial);

                if (isSpecial)
                {
                    rndVal = Random.value;

                    if (rndVal <= .33f)
                    {
                        result = CollectableType.Cofee;
                    }
                    else if (rndVal <= .66f)
                    {
                        result = CollectableType.Energy;
                    }
                    else
                    {
                        result = CollectableType.Shield;
                    }
                }
                else
                {
                    rndVal = Random.value;

                    if (rndVal < .05f && Blackboard.Version.Value > 1)
                    {
                        result = CollectableType.Fix;

                    }
                    else
                    {
                        result = CollectableType.Code;
                    }
                }
            }

            return result;
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
                if (_lastSpawned == null
                    || PositionProvider.Instance.ScreenTop.position.y - _lastSpawned.transform.position.y >= _spawnDistance)
                {
                    _spawnDistance = GetRandomSpawnDistance();
                    Spawn();
                }
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
                    * Time.deltaTime * Blackboard.MovementSpeed.Value * Blackboard.GameSpeedRatio.Value * Blackboard.GameSpeedRatioModifier.Value;

                if (collectable.transform.position.y + .5f < PlayerController.Instance.transform.position.y)
                {
                    collectable.HideByDistance();
                }

                if (collectable.transform.position.y < PositionProvider.Instance.ScreenBottom.position.y)
                {
                    _inactiveCollectables.Add(collectable);
                }
            }
        }

        private float GetRandomSpawnDistance()
        {
            var minSpawnDistance = _minSpawnDistanceCurve.Evaluate(Blackboard.Difficulty.Value);
            var maxSpawnDistance = _maxSpawnDistanceCurve.Evaluate(Blackboard.Difficulty.Value);
            return Random.Range(minSpawnDistance, maxSpawnDistance);
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

            Collectable.OnDeactivate.Subscribe(collectable =>
            {
                _activeCollectables.Remove(collectable);
            }).AddTo(this);
        }
    }
}
