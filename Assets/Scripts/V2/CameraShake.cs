using UnityEngine;

namespace V2
{
    /// <summary>
    /// Shake corto de cámara (M3): offset aleatorio con caída lineal y restauración exacta
    /// del localPosition original. Sin Cinemachine, sin timeScale, sin post-proceso.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float hitAmplitude = 0.12f;
        [SerializeField] private float hitDuration = 0.15f;
        [SerializeField] private float gameOverAmplitude = 0.18f;
        [SerializeField] private float gameOverDuration = 0.20f;

        private Vector3 baseLocalPosition;
        private float amplitude;
        private float duration;
        private float endTime;
        private bool active;
        private bool baseCaptured;

        /// <summary>True mientras hay un shake en curso (verificación).</summary>
        public bool IsShaking => active;

        private void Awake()
        {
            CaptureBase();
        }

        private void CaptureBase()
        {
            baseLocalPosition = transform.localPosition;
            baseCaptured = true;
        }

        /// <summary>Shake de impacto de la nave (0.12 u / 0.15 s por defecto).</summary>
        public void ShakeHit()
        {
            Shake(hitAmplitude, hitDuration);
        }

        /// <summary>Shake de GameOver (0.18 u / 0.20 s por defecto).</summary>
        public void ShakeGameOver()
        {
            Shake(gameOverAmplitude, gameOverDuration);
        }

        /// <summary>
        /// Arranca/refuerza un shake. Un shake más débil no interrumpe a uno más fuerte
        /// vigente (el golpe no pisa el GameOver ni viceversa).
        /// </summary>
        public void Shake(float newAmplitude, float newDuration)
        {
            if (newAmplitude <= 0f || newDuration <= 0f) return;
            if (!baseCaptured) CaptureBase();
            if (active && Time.time < endTime && newAmplitude < amplitude) return;

            amplitude = newAmplitude;
            duration = newDuration;
            endTime = Time.time + newDuration;
            active = true;
        }

        private void Update()
        {
            if (!active) return;

            if (Time.time >= endTime)
            {
                Restore();
                return;
            }

            float falloff = Mathf.Clamp01((endTime - Time.time) / duration);
            Vector2 offset = Random.insideUnitCircle * (amplitude * falloff);
            transform.localPosition = baseLocalPosition + new Vector3(offset.x, offset.y, 0f);
        }

        private void OnDisable()
        {
            if (active) Restore();
        }

        private void Restore()
        {
            active = false;
            transform.localPosition = baseLocalPosition;
        }
    }
}
