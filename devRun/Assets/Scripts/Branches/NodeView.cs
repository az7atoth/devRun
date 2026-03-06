using Az7.Utils.Pool;
using UnityEngine;

namespace DevRun
{
    public class NodeView : MonoBehaviour
    {
        public BranchesColorIndex CurrentColor { get; private set; }

        [SerializeField] private SpriteRenderer _outer;
        [SerializeField] private SpriteRenderer _middle;
        [SerializeField] private SpriteRenderer _inner;
        [SerializeField] private PoolableObject _poolableObject;

        public void Activate()
        {
            gameObject.SetActive(true);
            SetView(BranchesColorIndex.Yellow, Style.Player);
        }

        public void SetView(BranchesColorIndex index, Style style)
        {
            SetRendererColor(index, _outer);

            switch (style)
            {
                case Style.Player:
                    SetRendererColor(BranchesColorIndex.DarkGray, _middle);
                    SetRendererColor(BranchesColorIndex.White, _inner);
                    break;

                case Style.Main:
                    SetRendererColor(BranchesColorIndex.None, _middle);
                    SetRendererColor(BranchesColorIndex.DarkGray, _inner);
                    break;

                case Style.Common:
                    SetRendererColor(BranchesColorIndex.None, _middle);
                    SetRendererColor(BranchesColorIndex.White, _inner);
                    break;
            }
        }

        public void Deactivate()
        {
            if (_poolableObject != null)
            {
                _poolableObject.Return();
            }
            else
            {
                Debug.Log("Deactivate " + gameObject.name + " is failed. Destroy.");
                Destroy(gameObject);
            }
        }

        private void SetRendererColor(BranchesColorIndex index, SpriteRenderer target)
        {
            target.color = ColorProvider.Instance.Get(index);
        }

        public enum Style
        {
            Common,
            Player,
            Main,
        }
    }
}
