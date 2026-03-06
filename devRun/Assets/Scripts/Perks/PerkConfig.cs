using UnityEngine;

namespace DevRun
{
    [CreateAssetMenu(fileName = "PerkConfig", menuName = "Scriptable Objects/PerkConfig")]
    public class PerkConfig : ScriptableObject
    {
        [field: SerializeField] public PerkType PerkType { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; }
        [field: SerializeField] public float EffectByLevel { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}
