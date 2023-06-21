using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public struct MathExtentions
    {
        public static float Max(Vector3 value)
        {
            return Mathf.Max(value.x, Mathf.Max(value.y, value.z));
        }

        public static float Max(Vector2 value)
        {
            return Mathf.Max(value.x, value.y);
        }

        public static float Min(Vector3 value)
        {
            return Mathf.Min(value.x, Mathf.Min(value.y, value.z));
        }

        public static float Min(Vector2 value)
        {
            return Mathf.Min(value.x, value.y);
        }
    }
}
