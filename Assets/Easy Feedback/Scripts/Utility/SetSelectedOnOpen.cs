using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyFeedback
{
    [RequireComponent(typeof(FormElement))]
    public class SetSelectedOnOpen : MonoBehaviour
    {
        private FeedbackForm form;
        private Coroutine coroutine;


        private void Awake()
        {
            form = GetComponentInParent<FeedbackForm>();

            // register event handlers
            form.OnFormOpened.AddListener(startSelectedCoroutine);
            form.OnFormClosed.AddListener(stopCoroutineIfExists);
        }

        private void startSelectedCoroutine()
        {
            coroutine = StartCoroutine(SetSelfAsSelected());
        }

        private void stopCoroutineIfExists()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        IEnumerator SetSelfAsSelected()
        {
            // check if there is an eventsystem in the scene
            if (!EventSystem.current)
            {
                Debug.LogError("Scene is missing an EventSystem.");
                yield break;
            }

            // set self as selected
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);

            coroutine = null;
        }
    }
}