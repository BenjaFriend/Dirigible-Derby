using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFeedback.Demo
{
    public class LogMessage : MonoBehaviour
    {
        public void LogMessages()
        {
            // log a few different style of messages
            Debug.Log("Test Log");
            Debug.LogWarning("Test Warning");
            Debug.LogError("Test Error");
        }
    }
}