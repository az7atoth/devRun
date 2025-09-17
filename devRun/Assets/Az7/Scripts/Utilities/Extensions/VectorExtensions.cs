using UnityEngine;

namespace Az7.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 Convert3XZ(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0f, vector2.y);
        }

        public static Vector2 Convert2XZ(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
    }
}
