using UnityEngine;

namespace Branches
{
    public class MergeSelector : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private GameObject[] _icons;

        public void Show()
        {
            _cg.alpha = 1.0f;
        }

        public void Hide()
        {
            _cg.alpha = 0.0f;
        }

        public void SetIcon(SelectorState state)
        {
            switch (state)
            {
                case SelectorState.Question:
                    EnableIcon(0);
                    break;

                case SelectorState.Left:
                    EnableIcon(1);
                    break;

                case SelectorState.Right:
                    EnableIcon(2);
                    break;

                case SelectorState.Equal:
                    EnableIcon(3);
                    break;
            }
        }

        private void EnableIcon(int iconIndex)
        {
            for (int i = 0; i < _icons.Length; i++)
            {
                _icons[i].SetActive(i == iconIndex);
            }
        }

        public enum SelectorState
        {
            Question = 0,
            Left = 1,
            Right = 2,
            Equal = 3
        }

    }
}
