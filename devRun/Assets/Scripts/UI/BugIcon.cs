using UnityEngine;
using UnityEngine.UI;

namespace DevRun
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
