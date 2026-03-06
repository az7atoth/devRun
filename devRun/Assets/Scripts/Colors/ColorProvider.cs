using UnityEngine;

namespace DevRun
{
    public class ColorProvider : MonoBehaviour
    {
        public static ColorProvider Instance { get; private set; }

        [SerializeField] private ColorConfig[] _colorConfigs;
        [SerializeField] private int _minRandomColorIndex;
        [SerializeField] private int _maxRandomColorIndex;

        public Color Get(BranchesColorIndex index)
        {
            ColorConfig config = null;

            for (int i = 0; i < _colorConfigs.Length; i++)
            {
                if (_colorConfigs[i].Index == index)
                {
                    config = _colorConfigs[i];
                }
            }

            if (config != null)
            {
                return config.Color;
            }
            else
            {
                Debug.Log("Color Provider: color not founded - " + index.ToString());
                return Color.azure;
            }
        }

        //TODO modify
        public Color GetRandom(BranchesColorIndex exception = BranchesColorIndex.None)
        {
            var rndIndex = 0;
            ColorConfig config = null;
            var i = 0;

            while (i < 100)
            {
                rndIndex = Random.Range(_minRandomColorIndex, _maxRandomColorIndex + 1);
                config = _colorConfigs[rndIndex];
                if (config.Index == exception)
                {
                    i++;
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (config != null)
            {
                return config.Color;
            }
            else
            {
                Debug.Log("Color Provider: failed to pick random color");
                return Color.azure;
            }
        }

        private void Awake()
        {
            if (Instance == null)
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
