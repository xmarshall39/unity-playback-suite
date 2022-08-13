using System.Linq;
using UnityEngine;
namespace UPBS.Utility
{
    public static class HelperFunctions
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var result = new T[arrays.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < arrays.Length; ++x)
            {
                arrays[x].CopyTo(result, offset);
                offset += arrays[x].Length;
            }
            return result;
        }
        /// <summary>
        /// Used for interpreting simple classes/structs as viable FrameData headers so those headers can be generated
        /// </summary>
        public static string[] GenerateHeaderForClass(object o, string baseName)
        {
            switch (o)
            {
                case Vector3 _: return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z" };
                case Vector3Int _: return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z" };
                case Vector2 _: return new string[] { $"{baseName}_x", $"{baseName}_y" };
                case Vector2Int _: return new string[] { $"{baseName}_x", $"{baseName}_y" };
                case Quaternion _: return new string[] { $"{baseName}_x", $"{baseName}_y", $"{baseName}_z", $"{baseName}_w" };
                default:
                    Debug.LogWarning($"Provided Class [{o.GetType().Name}] not accounted for!");
                    return null;
            }
        }
    }
}

