using System;
using UnityEngine;

namespace MultiSuika.Utilities
{
    [Serializable]
    public class IntReference
    {
        public bool UseConstant = true;
        public int ConstantValue;
        public IntVariable Variable;

        public IntReference(int value = default, bool useConstant = false)
        {
            UseConstant = useConstant;
            ConstantValue = value;
            if (UseConstant) 
                return;
            Variable = ScriptableObject.CreateInstance<IntVariable>();
            Variable.SetValue(value);
        }

        public IntReference(int value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public int Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}