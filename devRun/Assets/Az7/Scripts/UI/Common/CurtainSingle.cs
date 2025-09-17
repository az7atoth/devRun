namespace Az7.UI
{
    public class CurtainSingle : Curtain
    {
        public static CurtainSingle Instance { get; private set; }

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
                return;
            }
        }
    }
}