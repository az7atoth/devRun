using TMPro;
using UnityEngine;
using UniRx;

namespace DevRun
{
    public class VersionCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _versionText;

        private void Awake()
        {
            Blackboard.Version.Subscribe(value =>
            {
                var t1 = value / 10;
                var t2 = value % 10;
                _versionText.text = t1 + "." + t2;
            }).AddTo(this);
        }
    }
}
