using System;
using UnityEngine;
namespace UPBS.Utility
{
    public static class ExtensionMethods
    {
        public static T[] Concat<T>(this T[] a, T[] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            int oldLen = a.Length;
            Array.Resize<T>(ref a, a.Length + b.Length);
            Array.Copy(b, 0, a, oldLen, b.Length);
            return a;
        }

        public static string ErrorVal(this int i) => "-1";
        public static string ErrorVal(this float f) => "-1.0";
        public static string[] ErrorVals(this Vector3 v) => new string[] { "-1.0", "-1.0", "-1.0" };

        #region Class Headers
        public static string[] Header(this Vector3 v, string baseName)
        {
            return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z" };
        }
        public static string[] Header(this Vector3Int v, string baseName)
        {
            return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z" };
        }
        public static string[] Header(this Vector2 v, string baseName)
        {
            return new string[] { $"{baseName}_x", $"{baseName}_y" };
        }
        public static string[] Header(this Vector2Int v, string baseName)
        {
            return new string[] { $"{baseName}_x", $"{baseName}_y" };
        }
        public static string[] Header(this Quaternion v, string baseName)
        {
            return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z", $"{baseName}_w" };
        }
        public static string[] Header(this Matrix4x4 v, string baseName)
        {
            return new string[] { 
                $"{baseName}_0_0", $"{baseName}_0_1", $"{baseName}_0_2", $"{baseName}_0_3",
                $"{baseName}_1_0", $"{baseName}_1_1", $"{baseName}_1_2", $"{baseName}_1_3",
                $"{baseName}_2_0", $"{baseName}_2_1", $"{baseName}_2_2", $"{baseName}_2_3",
                $"{baseName}_3_0", $"{baseName}_3_1", $"{baseName}_3_2", $"{baseName}_3_3"
            };
        }

        #endregion
    }
}

