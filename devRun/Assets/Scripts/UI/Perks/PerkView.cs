using TMPro;
using UnityEngine;

namespace Branches
{
    public class PerkView : MonoBehaviour
    {
        [field: SerializeField] public PerkType PerkType { get; private set; }

        [SerializeField] private TMP_Text _levelText;

        public void SetLevel(int level)
        {
            _levelText.text = level.ToString();
        }
    }
}
