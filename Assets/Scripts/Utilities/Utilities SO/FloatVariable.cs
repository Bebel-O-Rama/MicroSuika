using UnityEngine;

namespace MultiSuika.Utilities
{
    [CreateAssetMenu(menuName = "Utilities/Float Variable")]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public float Value;

        public bool resetOnEnable = false;

        public void SetValue(float value)
        {
            Value = value;
        }

        public void SetValue(FloatVariable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(float amount)
        {
            Value += amount;
        }
        
        public void ApplyChangeClamp(float amount, float min = Mathf.NegativeInfinity, float max = Mathf.Infinity)
        {
            Value = Mathf.Clamp(Value + amount, min, max);
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
        }

        private void OnEnable()
        {
            if (resetOnEnable)
                Value = 0f;
        }
    }
}
