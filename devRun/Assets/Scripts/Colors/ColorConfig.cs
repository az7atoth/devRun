using UnityEngine;

namespace DevRun
{
    [CreateAssetMenu(fileName = "ColorConfig", menuName = "Scriptable Objects/ColorConfig")]
    public class ColorConfig : ScriptableObject
    {
        [field: SerializeField] public Color Color;
        [field: SerializeField] public BranchesColorIndex Index;
    }
}
