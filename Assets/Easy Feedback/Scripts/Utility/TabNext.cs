using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyFeedback
{
    public class TabNext : MonoBehaviour
    {
        public InputField Next;
        public InputField Previous;

        // Update is called once per frame
        void Update()
        {
            if (Next != null && this.GetComponent<InputField>().isFocused
                && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                && Input.GetKeyDown(KeyCode.Tab))
            {
                GetComponent<InputField>().DeactivateInputField();
                Next.Select();
                Next.ActivateInputField();
            }
            else if (Previous != null && this.GetComponent<InputField>().isFocused
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                && Input.GetKeyDown(KeyCode.Tab))
            {
                GetComponent<InputField>().DeactivateInputField();
                Previous.Select();
                Previous.ActivateInputField();
            }
        }
    }
}