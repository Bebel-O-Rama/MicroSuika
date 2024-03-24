using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public static class UnityExtension
    {
        public static void SetLayerRecursively(this Transform parent, int layer)
        {
            parent.gameObject.layer = layer;
 
            for (int i = 0, count = parent.childCount; i < count; i++)
            {
                parent.GetChild(i).SetLayerRecursively(layer);
            }
        }
        
        public static void AddUnique<T>(this List<T> list, T obj)
        {
            if (list.Contains(obj))
                return;
            list.Add(obj);
        }
        
        public static void SetInList<T>(this List<T> list, T obj, bool isAdding = true)
        {
            if (isAdding)
                list.Add(obj);
            else
            {
                list.Remove(obj);
            }
        }
        
        public static T GetElementAtIndexOrDefault<T>(this List<T> list, int index) => index >= 0 && index < list.Count ? list[index] : default;
        
        public static Vector3 WorldToLocalPosition(Transform relativeTargetTransform, Vector3 worldPosition) =>
            relativeTargetTransform.InverseTransformPoint(worldPosition);
        
        public static void ResetLocalTransform(this Transform child)
        {
            child.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            child.localScale = Vector3.one;
        }
        
        public static int DivideIntRoundedUp(int a, int b) => a / b + (a % b > 0 ? 1 : 0);
    }
}
