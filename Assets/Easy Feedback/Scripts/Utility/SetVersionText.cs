using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyFeedback
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class SetVersionText : MonoBehaviour
    {
        public string VersionNumber;
        public string Prefix, Suffix;
        // Use this for initialization
        void Start()
        {
            // set version text
            Text text = GetComponent<Text>();
            text.text = Prefix + VersionNumber + Suffix;
        }
    }
}