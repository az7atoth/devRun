using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Branches
{
    public class PerkViewController : MonoBehaviour
    {
        [SerializeField] private PerkView[] _views;

        private Dictionary<PerkType, PerkView> _viewsDict = new();

        private void UpdateView()
        {
            for (int i = 0; i < _views.Length; i++)
            {
                _views[i].gameObject.SetActive(false);
            }

            foreach (var item in PerkController.Instance.ActivePerks)
            {
                if (_viewsDict.TryGetValue(item.Key, out var view))
                {
                    view.gameObject.SetActive(true);
                    view.SetLevel(item.Value);
                }
            }
        }

        private void Awake()
        {
            for (int i = 0; i < _views.Length; i++)
            {
                _viewsDict.Add(_views[i].PerkType, _views[i]);
            }
        }

        private void Start()
        {
            PerkController.Instance.OnPerksChanged.Subscribe(_ =>
            {
                UpdateView();
            }).AddTo(this);
        }
    }
}
