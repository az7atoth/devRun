namespace Az7.UI
{
    public class UI_ControllerSingle : UI_Controller
    {
        public static UI_ControllerSingle Instance { get; private set; }

        protected override void AwakeBase()
        {
            base.AwakeBase();

            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
