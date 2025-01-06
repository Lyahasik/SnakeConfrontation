using UnityEngine;
using System.Collections;

namespace SnakeWarzIO
{
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker instance;

        private Transform camTransform;
        internal float decreaseFactor = 1.0f;
        private Vector3 originalPos;
        private float p = 0;
        private bool isShaking;

        void Awake()
        {
            instance = this;
            isShaking = false;
            camTransform = GetComponent(typeof(Transform)) as Transform;
            originalPos = camTransform.localPosition;
        }

        private void Update()
        {
            //Debug the shake intensity
            if (Application.isEditor && Input.GetKeyDown(KeyCode.S))
                PublicShake(1.2f, 0.5f);
        }

        /// <summary>
        /// Main shake logic. 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public IEnumerator ShakeCo(float duration, float power)
        {
            if (isShaking)
                yield break;
            isShaking = true;

            originalPos = camTransform.localPosition;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime * decreaseFactor;
                p = Mathf.SmoothStep(power, 0.01f, t / duration);
                transform.localPosition = new Vector3(
                    originalPos.x + Random.insideUnitSphere.x * p,
                    originalPos.y + Random.insideUnitSphere.y * p,
                    transform.localPosition.z);

                yield return 0;
            }

            if (t >= duration)
            {
                transform.localPosition = new Vector3(originalPos.x, originalPos.y, transform.localPosition.z);
                isShaking = false;
            }
        }

        /// <summary>
        /// Use this to ask for a shake in a normal way (no shake will be done if another shake is in progress)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="p"></param>
        public void PublicShake(float d, float p)
        {
            StartCoroutine(ShakeCo(d, p));
        }

        /// <summary>
        /// Use this to ask for a shake in a forced way (shake will be done even if another shake is playing already)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="p"></param>
        public void ForceShake(float d, float p)
        {
            isShaking = false;
            StartCoroutine(ShakeCo(d, p));
        }

    }
}