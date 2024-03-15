using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private List<ScoreModifiers> _currentSpeedModifiers;
        private List<ScoreInformation> _scoreInformation;

        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static ScoreManager Instance => _instance ??= new ScoreManager();

        private static ScoreManager _instance;

        private ScoreManager()
        {
        }

        #endregion
        
        // For now, let's hardset that to 4
        private void Awake()
        {
            _instance = this;
            
            _scoreInformation = new List<ScoreInformation>();
            for (int i = 0; i <= 4; i++)
            {
                _scoreInformation.Add(new ScoreInformation());
            }
        }

        public IntReference GetPlayerScoreReference(int playerIndex) => _scoreInformation[playerIndex].scoreReference;
        
        // Find a better way to generalize all that
        public List<IntReference> GetPlayerScoreReferences() => _scoreInformation.Select(s => s.scoreReference).ToList();
        public void ResetScoreInformation() => _scoreInformation.ForEach(s => s.ResetScoreInformation());
    }

    public class ScoreInformation
    {
        public readonly IntReference scoreReference;
        public readonly IntReference comboReference;
        public readonly FloatReference speedReference;

        public ScoreInformation()
        {
            scoreReference = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            
            comboReference = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            
            speedReference = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
        }

        public void ResetScoreInformation()
        {
            scoreReference.Variable.SetValue(0);
            comboReference.Variable.SetValue(0);
            speedReference.Variable.SetValue(0);
        }
    }
    
    
    
    
}
