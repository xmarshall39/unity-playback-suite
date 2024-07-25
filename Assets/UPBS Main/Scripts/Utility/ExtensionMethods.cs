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

        public static T[] Concat<T>(this T[] a, params T[][] bs)
        {
            if (a == null) throw new ArgumentNullException("a");
            int progLen = a.Length;
            int fullLength = a.Length;
            foreach (var b in bs) fullLength += b.Length;
            Array.Resize<T>(ref a, fullLength);
            
            foreach (var b in bs)
            {
                if (b == null) throw new ArgumentNullException("b");
                Array.Copy(b, 0, a, progLen, b.Length);
                progLen += b.Length;
            }

            return a;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
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
        public static string[] Header(this Color v, string baseName)
        {
            return new string[] { $"{baseName}_r", $"{baseName}_g", $"{baseName}_b", $"{baseName}_a" };
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


        public static string[] HeaderAppends(this Vector3 _)
        {
            return new string[] { "x", "y", "z" };
        }
        public static string[] HeaderAppends(this Vector3Int _)
        {
            return new string[] { "x", "y", "z" };
        }
        public static string[] HeaderAppends(this Vector2 _)
        {
            return new string[] { "x", "y" };
        }
        public static string[] HeaderAppends(this Vector2Int _)
        {
            return new string[] { "x", "y" };
        }
        public static string[] HeaderAppends(this Quaternion _)
        {
            return new string[] { "x", "y", "z", "w" };
        }
        public static string[] HeaderAppends(this Color _)
        {
            return new string[] { "r", "g", "b", "a" };
        }

        public static string[] HeaderAppends(this Matrix4x4 _)
        {
            return new string[] {
                "0_0", "0_1", "0_2", "0_3",
                "1_0", "1_1", "1_2", "1_3",
                "2_0", "2_1", "2_2", "2_3",
                "3_0", "3_1", "3_2", "3_3"
            };
        }

        #endregion
    }
}

