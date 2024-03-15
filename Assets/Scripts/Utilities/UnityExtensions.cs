using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public static class UnityExtensions
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
    }
}
