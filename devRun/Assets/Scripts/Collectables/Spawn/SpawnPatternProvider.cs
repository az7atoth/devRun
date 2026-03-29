using UnityEngine;
using Zenject;
using UniRx;
using System.Text;

namespace DevRun
{
    public class SpawnPatternProvider : MonoBehaviour, IInitializable
    {
        [SerializeField] private PatternBasedSpawnerConfig _earlyGameConfig;
        [SerializeField] private PatternBasedSpawnerConfig _midGameConfig;
        [SerializeField] private PatternBasedSpawnerConfig _lateGameConfig;

        [SerializeField] private int _midGameLevel;
        [SerializeField] private int _lateGameLevel;

        private GamePhase _currentPhase;

        private SpawnPatternData[] _earlyPatterns;
        private SpawnPatternData[] _midPatterns;
        private SpawnPatternData[] _latePatterns;

        private int _lastPatternId;
        private StringBuilder _sb = new StringBuilder(25);

        public void Initialize()
        {
            _earlyPatterns = ProcessConfig(_earlyGameConfig);
            _midPatterns = ProcessConfig(_midGameConfig);
            _latePatterns = ProcessConfig(_lateGameConfig);

            Blackboard.Level.SkipLatestValueOnSubscribe()
                .Subscribe(version => DefineGamePhase(version)).AddTo(this); //TODO
        }

        public string GetPattern(int lineLength)
        {
            SpawnPatternData[] patternsPool;

            switch (_currentPhase)
            {
                case GamePhase.Middle:
                    patternsPool = _midPatterns;
                    break;

                case GamePhase.Late:
                    patternsPool = _latePatterns;
                    break;

                default:
                    patternsPool = _earlyPatterns;
                    break;
            }

            var rnd = Random.Range(0, patternsPool.Length);

            if (patternsPool.Length > 1)
            {
                while (rnd == _lastPatternId)
                {
                    rnd = Random.Range(0, patternsPool.Length);
                }
            }

            _lastPatternId = rnd;

            var pattern = patternsPool[rnd];

            _sb.Clear();

            for (int i = 0; i < pattern.Lines.Length; i++)
            {
                var line = pattern.Lines[i];

                if (line.Length >= lineLength)
                {
                    _sb.Append(line, 0, lineLength);
                }
                else
                {
                    var difference = lineLength - line.Length;
                    _sb.Append(line);
                    _sb.Append('.', difference);
                }
            }

            return _sb.ToString();
        }

        private class SpawnPatternData
        {
            public string[] Lines;
        }

        public void DefineGamePhase(int level)
        {
            if (level < _midGameLevel)
            {
                _currentPhase = GamePhase.Early;
            }
            else if (level >= _midGameLevel && level < _lateGameLevel)
            {
                _currentPhase = GamePhase.Middle;
            }
            else
            {
                _currentPhase = GamePhase.Late;
            }
        }

        private SpawnPatternData[] ProcessConfig(PatternBasedSpawnerConfig config)
        {
            var configs = config.GetConfigs();

            var result = new SpawnPatternData[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                result[i] = new SpawnPatternData();
                result[i].Lines = configs[i].GetLines();
            }

            return result;
        }

        private enum GamePhase
        {
            Early,
            Middle,
            Late
        }
    } 
}
