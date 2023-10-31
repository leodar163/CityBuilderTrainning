using System.Collections;
using UnityEngine;

namespace UI.Animations
{
    public class UITranslation : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private AnimationCurve _translationCurve;
        [SerializeField] private bool _timeScaleSensitive;
        [SerializeField] private float _speed = 10;
        [SerializeField] private Vector2 _targetOffset;
        private Vector3 _originPosition;
        private Vector3 _targetPosition;
        private IEnumerator _translationCoroutine;
        private bool _isInitialized;
        private float _time;
        [Space] 
        [SerializeField] private bool debug = true;

        private void OnDrawGizmos()
        {
            if (!debug ||!_rectTransform || Application.isPlaying || !Application.isEditor) return;
            Gizmos.DrawWireCube((Vector2)_rectTransform.position + _targetOffset, 
                _rectTransform.rect.size);
            
            Gizmos.DrawSphere(_rectTransform.position + (Vector3)_targetOffset, 10f);
        }

        private void Awake()
        {
            _originPosition = _rectTransform.position;
            _targetPosition = _originPosition + (Vector3)_targetOffset;
        }

        public void TranslateToTarget()
        {
            Translate(true);
        }

        public void TranslateToOrigin()
        {
            Translate(false);
        }

        private void Translate(bool toTarget)
        {
            if (_translationCoroutine != null)
            {
                StopCoroutine(_translationCoroutine);
            }
            
            _translationCoroutine = TranslateRoutine(toTarget);

            StartCoroutine(_translationCoroutine);   
        }

        private IEnumerator TranslateRoutine(bool toTarget)
        {
            while (toTarget ? _time < 1 : _time > 0)
            {
                _time += _speed * (_timeScaleSensitive ? Time.deltaTime : 0.01f) * (toTarget ? 1 : -1);

                _time = Mathf.Clamp01(_time);
                
                _rectTransform.position = Vector3.Lerp(_originPosition, _targetPosition, 
                    _translationCurve.Evaluate(_time));
                
                yield return new WaitForEndOfFrame();
            }

            _translationCoroutine = null;
        }
    }
}