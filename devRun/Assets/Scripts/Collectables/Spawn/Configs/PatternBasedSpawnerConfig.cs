using System;
using UnityEngine;

namespace DevRun
{
    [CreateAssetMenu(fileName = "PatternBasedSpawnerConfig", menuName = "Scriptable Objects/PatternBasedSpawnerConfig")]
    public class PatternBasedSpawnerConfig : ScriptableObject
    {
        [SerializeField] private SpawnPatternConfig[] _patternConfigs;

        public SpawnPatternConfig[] GetConfigs()
        {
            if (_patternConfigs == null || _patternConfigs.Length == 0) return null;

            var result = new SpawnPatternConfig[_patternConfigs.Length];

            Array.Copy(_patternConfigs, result, _patternConfigs.Length);

            return result;
        }
    } 
}

