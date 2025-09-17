using UnityEngine;

namespace Az7.UI
{
    public class UI_ViewCommon : UI_ViewBase
    {
        public override UI_ViewKey ViewKey => _viewKey;
        [SerializeField] private UI_ViewKey _viewKey;
    }
}
