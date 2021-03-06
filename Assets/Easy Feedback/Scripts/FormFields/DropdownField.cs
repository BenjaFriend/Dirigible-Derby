﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback.FormFields
{
    /// <summary>
    /// A basic implementation of a dropdown field
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    class DropdownField : FormField
    {
        [Tooltip("The label to prepend to this field on the report (won't be included if left blank)")]
        public string Label;

        private Dropdown sourceField;

        public override void Awake()
        {
            base.Awake();

            // get source field
            sourceField = GetComponent<Dropdown>();
        }

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
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
                sectionText = sourceField.captionText.text;
            }
            else
            {
                sectionText = string.Format("{0}: {1}", Label, sourceField.captionText.text);
            }

            Form.CurrentReport[SectionTitle].SetText(sectionText);
            Form.CurrentReport[SectionTitle].SortOrder = SortOrder;
        }
    }
}
