using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiSuika.Utilities
{
    [Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;
        public float ConstantValue;
        public bool UseRandomRange = false;
        [Range(0f, 1f)] public float RandomCoefficient = 0f;
        public FloatVariable Variable;

        public FloatReference(float value = default, bool useConstant = false)
        {
            UseConstant = useConstant;
            ConstantValue = value;
            if (UseConstant) 
                return;
            Variable = ScriptableObject.CreateInstance<FloatVariable>();
            Variable.SetValue(value);
        }

        public float Value
        {
            get
            {
                if (!UseRandomRange || RandomCoefficient < Mathf.Epsilon)
                    return UseConstant ? ConstantValue : Variable.Value;
                return UseConstant
                    ? Random.Range(ConstantValue - ConstantValue * RandomCoefficient,
                        ConstantValue + ConstantValue * RandomCoefficient)
                    : Random.Range(Variable.Value - Variable.Value * RandomCoefficient,
                        Variable.Value + Variable.Value * RandomCoefficient);
            }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}