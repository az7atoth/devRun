using UnityEngine;

namespace Az7.Utils
{
    public static class WeightedRandom
    {
        public static int GetWeightIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                return -1;
            }

            var sum = 0f;

            foreach (var weight in weights)
            {
                sum += weight;
            }

            var rndVal = Random.Range(0f, sum);
            var wRef = 0f;

            var i = 0;

            foreach (var weight in weights)
            {
                wRef += weight;

                if (wRef >= rndVal)
                {
                    return i;
                }

                if (i == weights.Length - 1)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }

            return i;
        }
    } 
}
