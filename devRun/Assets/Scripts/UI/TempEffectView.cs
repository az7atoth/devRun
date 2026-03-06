using UnityEngine;
using UnityEngine.UI;

namespace DevRun
{
    public class TempEffectView : MonoBehaviour
    {
        [field: SerializeField] public Image Fill { get; private set; }
        [field: SerializeField] public CollectableType Type { get; private set; }
    }
}
