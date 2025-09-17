using UnityEngine;
using UnityEngine.UI;

namespace Branches
{
    public class BugIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public void SetActive(bool active)
        {
            _icon.gameObject.SetActive(active);
        }
    }
}
