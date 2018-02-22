using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFeedback.Demo
{
    public class CauseException : MonoBehaviour
    {
        public void ThrowException()
        {
            throw new System.Exception("Test");
        }
    }
}
