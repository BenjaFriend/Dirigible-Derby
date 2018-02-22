using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class FeedbackText : MonoBehaviour
    {
        public string Message = "Press {0} to submit feedback.";
        public FeedbackForm Form;
        public Text Text;

        private KeyCode currentKey;

        // Update is called once per frame
        void Update()
        {
            if (Form != null && currentKey != Form.FeedbackKey)
                updateText();
        }

        private void updateText()
        {
            currentKey = Form.FeedbackKey;
            Text.text = string.Format(Message, Form.FeedbackKey);
        }
    }
}