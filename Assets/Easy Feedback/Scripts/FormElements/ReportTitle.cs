using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    [RequireComponent(typeof(InputField))]
    public class ReportTitle : FormElement
    {
        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            Form.CurrentReport.Title = GetComponent<InputField>().text;
        }
    }

}