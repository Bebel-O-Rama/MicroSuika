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

        public static Vector3 WorldToLocalPosition(Transform relativeTargetTransform, Vector3 worldPosition) =>
            relativeTargetTransform.InverseTransformPoint(worldPosition);

        public static void ResetLocalTransform(this Transform child)
        {
            child.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            child.localScale = Vector3.one;
        }

        public static float EvaluateClamp(this AnimationCurve animeCurve, float time)
        {
            return animeCurve.length == 0
                ? 0f
                : animeCurve.Evaluate(Mathf.Clamp(time, animeCurve[0].time, animeCurve[animeCurve.length - 1].time));
        }
    }
}