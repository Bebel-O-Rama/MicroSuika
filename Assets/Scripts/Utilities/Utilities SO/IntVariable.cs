using UnityEngine;

namespace MultiSuika.Utilities
{
    [CreateAssetMenu(menuName = "Utilities/Int Variable")]
    public class IntVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public int Value;

        public bool resetOnEnable = false;

        public void SetValue(int value)
        {
            Value = value;
        }

        public void SetValue(IntVariable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(int amount)
        {
            Value += amount;
        }

        public void ApplyChange(IntVariable amount)
        {
            Value += amount.Value;
        }
    
        private void OnEnable()
        {
            if (resetOnEnable)
                Value = 0;
        }
    }
}
