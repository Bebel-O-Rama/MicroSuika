using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public abstract class ScoreModifiers: ScriptableObject
    {
        public string scoreModifierName;
        private FloatReference _mainScoreElement;

        public abstract void Init();
        
        public abstract FloatReference UpdateScore();
        
        

    }
}
