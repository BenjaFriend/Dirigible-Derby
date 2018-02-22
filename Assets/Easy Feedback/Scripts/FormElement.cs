using UnityEngine;

namespace EasyFeedback
{
    public abstract class FormElement : MonoBehaviour
    {
        /// <summary>
        /// The feedback form this component is a part of
        /// </summary>
        protected FeedbackForm Form;

        /// <summary>
        /// Called when the form is first opened, right before it is shown on screen
        /// </summary>
        public abstract void FormOpened();

        /// <summary>
        /// Called right before the report is sent to Trello
        /// </summary>
        /// <remarks>
        /// Add user-provided data to your report here
        /// </remarks>
        public abstract void FormSubmitted();

        /// <summary>
        /// Called when the form is closed, whether or not it was submitted
        /// </summary>
        public abstract void FormClosed();

        public virtual void Awake()
        {
            // find form in parent(s)
            Form = GetComponentInParent<FeedbackForm>();
            if (!Form)
            {
                Debug.LogError("This field is not part of a Feedback Form!");
            }

            // register events
            Form.OnFormOpened.AddListener(FormOpened);
            Form.OnFormSubmitted.AddListener(FormSubmitted);
            Form.OnFormClosed.AddListener(FormClosed);
        }

    }
}
