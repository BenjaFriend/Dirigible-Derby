using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    /// <summary>
    /// Attaches the debug log to the report as as text file
    /// </summary>
    class DebugLogCollector : FormElement
    {
        private StringBuilder log;

        public override void Awake()
        {
            base.Awake();

            // instantiate string builder
            log = new StringBuilder();

            // register Application log callback
            Application.logMessageReceived += new Application.LogCallback(handleLog);
        }

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            // attach log
            byte[] bytes = Encoding.ASCII.GetBytes(log.ToString());
            Form.CurrentReport.AttachFile("log.txt", bytes);
        }

        private void handleLog(string logString, string stackTrace, LogType logType)
        {
            // enqueue the message
            if (logType != LogType.Exception)
            {
                log.AppendFormat("{0}: {1}", logType.ToString(), logString);
            }
            else
            {
                // don't add log type to exceptions, as it's already in the string
                log.AppendLine(logString);
            }

            // enqueue the stack trace
            log.AppendLine(stackTrace);
        }
    }

}