using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback.Demo
{
    [RequireComponent(typeof(Text))]
    public class CallbackTest : MonoBehaviour
    {
        public float FadeTime = 2f;

        private Text text;
        private Coroutine coroutine;

        void Start()
        {
            text = GetComponent<Text>();
            setAlpha(0f);
        }

        private void setAlpha(float a)
        {
            Color c = text.color;
            c.a = a;
            text.color = c;
        }

        /// <summary>
        /// Called by an event on the Feedback Form.
        /// </summary>
        public void OnEvent()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(fadeCoroutine());
        }

        IEnumerator fadeCoroutine()
        {
            float a = 1f;

            do
            {
                setAlpha(a);
                a -= Time.deltaTime / FadeTime;
                yield return new WaitForEndOfFrame();
            } while (a > 0);
        }
    }
}