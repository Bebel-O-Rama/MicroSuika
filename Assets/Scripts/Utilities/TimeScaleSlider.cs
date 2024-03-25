using UnityEngine;

namespace MultiSuika.Utilities
{
    public class TimeScaleSlider : MonoBehaviour
    {
        [SerializeField] [Range (0f, 2f)] private float _timeScaleSlider = 1f;

        private float _fixedDeltaTime;

        private void Awake()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;
            _timeScaleSlider = Time.timeScale;
        }

        private void Update()
        {
            Time.timeScale = _timeScaleSlider;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }
    }
}
