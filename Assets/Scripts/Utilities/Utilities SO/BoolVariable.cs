using UnityEngine;

namespace MultiSuika.Utilities
{
    [CreateAssetMenu(menuName = "Utilities/Bool Variable")]
    public class BoolVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public bool Value;

        public bool resetOnEnable = false;

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariable value)
        {
            Value = value.Value;
        }

        public void ChangeValue()
        {
            Value = !Value;
        }

        private void OnEnable()
        {
            if (resetOnEnable)
                Value = false;
        }
    }
}
