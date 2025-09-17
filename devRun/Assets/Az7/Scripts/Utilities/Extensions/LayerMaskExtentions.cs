using UnityEngine;

namespace Az7.Extensions
{
    public static class LayerMaskExtentions
    {
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }

        public static bool Contains(this LayerMask layerMask, LayerMask another)
        {
            return (layerMask.value & another.value) != 0;
        }

        public static void Add(ref this LayerMask layerMask, int layer)
        {
            layerMask.value = layerMask | (1 << layer);
        }

        public static void Add(ref this LayerMask layerMask, LayerMask another)
        {
            layerMask.value = layerMask | another;
        }

        public static void Remove(ref this LayerMask layerMask, int layer)
        {
            layerMask.value = layerMask & ~(1 << layer);
        }

        public static void Remove(ref this LayerMask layerMask, LayerMask another)
        {
            layerMask.value = layerMask & ~another;
        }
    }
}
