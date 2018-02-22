using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback.FormFields
{
    /// <summary>
    /// A basic implementation of a text field
    /// </summary>
    [RequireComponent(typeof(InputField))]
    class TextField : FormField
    {
        [Tooltip("The label to prepend to this field on the report (won't be included if left blank)")]
        public string Label;

        private InputField sourceField;
        private string lastValue;

        public override void Awake()
        {
            base.Awake();

            // get source field
            sourceField = GetComponent<InputField>();

            // register onchagned callback
            sourceField.onValueChanged.AddListener(onValueChanged);
        }

        /// <summary>
        /// Preserves current state of the InputField, as esc clears
        /// the text by default, and can't be overridden
        /// </summary>
        private void onValueChanged(string val)
        {
            if (!Input.GetKey(KeyCode.Escape))
            {
                lastValue = val;
            }
        } 

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
            // restore value before form was closed
            sourceField.text = lastValue;
        }

        public override void FormSubmitted()
        {
            // add section to the form
            if (string.IsNullOrEmpty(SectionTitle))
                throw new NullReferenceException("The section title for this field is not set!");

            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);
            else
            {
                Debug.LogWarning("The section " + SectionTitle + " already exists! Overwriting.");
            }

            // set section text
            string sectionText;
            if (string.IsNullOrEmpty(Label))
            {
                sectionText = sourceField.text;
            }
            else
            {
                sectionText = string.Format("{0}: {1}", Label, sourceField.text);
            }

            Form.CurrentReport[SectionTitle].SetText(sectionText);
            Form.CurrentReport[SectionTitle].SortOrder = SortOrder;

            // clear field
            lastValue = string.Empty;
        }
    }
}
