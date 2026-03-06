using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPattern", menuName = "Scriptable Objects/SpawnPattern")]
public class SpawnPatternConfig : ScriptableObject
{
    [SerializeField] private string[] _lines;

    public string[] GetLines()
    {
        if (_lines == null || _lines.Length == 0) return null;

        var result = new string[_lines.Length];

        Array.Copy(_lines, result, _lines.Length);

        return result;
    }
}
