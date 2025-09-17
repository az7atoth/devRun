using UnityEngine;

namespace Az7.Utils
{
    public static class EaseFunctions
    {
        public static float GetValue(EaseType type, float x)
        {
            switch (type)
            {
                case EaseType.Sine: return EaseInOutSine(x);
                case EaseType.Quad: return EaseInOutQuad(x);
                case EaseType.Cubic: return EaseInOutCubic(x);
                default: return x;
            }
        }

        public static float EaseInOutSine(float x)
        {
            x = Mathf.Clamp01(x);
            return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        }

        public static float EaseInOutQuad(float x)
        {
            x = Mathf.Clamp01(x);
            return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
        }

        public static float EaseInOutCubic(float x)
        {
            x = Mathf.Clamp01(x);
            return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
        }
    }

    public enum EaseType { None, Sine, Quad, Cubic }

}