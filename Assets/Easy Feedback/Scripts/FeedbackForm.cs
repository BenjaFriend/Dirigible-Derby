using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using EasyFeedback.APIs;
using UnityEngine.Events;

namespace EasyFeedback
{
    public class FeedbackForm : MonoBehaviour
    {
        [Tooltip("Easy Feedback configuration file")]
        public EFConfig Config;

        [Tooltip("Key to toggle feedback window")]
        public KeyCode FeedbackKey = KeyCode.F12;

        [Tooltip("Include screenshot with reports?")]
        public bool IncludeScreenshot = true;

        public Transform Form, Alert;

        /// <summary name="OnFormOpened">
        /// Called when the form is first opened, right before it is shown on screen
        /// </summary>
        [Tooltip("Functions to be called when the form is first opened")]
        public UnityEvent OnFormOpened;

        /// <summary name="OnFormSubmitted">
        /// Called right before the report is sent to Trello
        /// </summary>
        [Tooltip("Functions to be called when the form is submitted")]
        public UnityEvent OnFormSubmitted; // called before report is sent to Trello, so that any information in the form can be added

        /// <summary name="OnFormClosed">
        /// Called when the form is closed, whether or not it was submitted
        /// </summary>
        [Tooltip("Functions to be called when the form is closed")]
        public UnityEvent OnFormClosed;


        /// <summary>
        /// The current report being built.
        /// Will be sent as next report
        /// </summary>
        public Report CurrentReport;

        /// <summary>
        /// Whether or not the form is currently being displayed
        /// </summary>
        [HideInInspector]
        public bool IsOpen
        {
            get { return Form.gameObject.activeSelf; }
        }

        private Text alertText;

        private Coroutine ssCoroutine;

        // form metadata
        private string screenshotPath;

        // api handler
        APIs.Trello trello;

        private bool initCursorVisible;
        private CursorLockMode initCursorLockMode;

        private bool submitting;


        public void Awake()
        {
            if (!Config.StoreLocal)
                InitTrelloAPI();

            // initialize current report
            initCurrentReport();
        }

        public void InitTrelloAPI()
        {
            // initialize api handler
            trello = new APIs.Trello(Config.Token);
        }

        /// <summary>
        /// Replaces currentReport with a new instance of Report
        /// </summary>
        private void initCurrentReport()
        {
            CurrentReport = new Report();
        }

        // Use this for initialization
        void Start()
        {
            // get alert elements
#if UNITY_2017_3
            alertText = Alert.Find("Text").GetComponent<Text>();
#else
            alertText = Alert.FindChild("Text").GetComponent<Text>();
#endif
        }

        // Update is called once per frame
        void Update()
        {
            // show form when player hits F12
            if (Input.GetKeyDown(FeedbackKey)
                && !IsOpen
                && ssCoroutine == null)
            {
                this.Show();
            }
            else if ((Input.GetKeyDown(FeedbackKey) || Input.GetKeyDown(KeyCode.Escape)) // close form if f12 is hit again,  or escape is hit
                && IsOpen
                && !submitting)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Takes a screenshot, then opens the form
        /// </summary>
        public void Show()
        {
            if (!IsOpen && ssCoroutine == null)
                ssCoroutine = StartCoroutine(screenshotAndOpenForm());
        }

        /// <summary>
        /// Called by the submit button, submits the form.
        /// </summary>
        public void Submit()
        {
            StartCoroutine(submitAsync());
        }

        private void showAlert(string message)
        {
            alertText.text = message;
            Alert.gameObject.SetActive(true);

            releaseMouse();
        }

        public void HideAlert()
        {
            hideMouse();

            Alert.gameObject.SetActive(false);
        }

        private IEnumerator submitAsync()
        {
            // disable form
            DisableForm();

            submitting = true;

            // show submitting alert
            showAlert("Submitting...");

            // call OnFormSubmitted
            OnFormSubmitted.Invoke();


            if (!Config.StoreLocal)
            {
                // add card to board
                yield return trello.AddCard(
                    CurrentReport.Title,
                    CurrentReport.ToString(),
                    CurrentReport.Label.id ?? "",
                    CurrentReport.List.id,
                    CurrentReport.Screenshot);

                // send up attachments 
                if (trello.LastAddCardResponse != null && !trello.UploadError)
                    yield return attachFilesAsync(trello.LastAddCardResponse.id);
            }
            else
            {
                // store entire report locally, then return
                string localPath = writeLocal(CurrentReport);
                Debug.Log(localPath);
            }

            // close form
            this.Hide();

            if (!Config.StoreLocal && trello.UploadError)
            {
                // preserve report locally if there's an issue during upload
                Debug.Log(writeLocal(CurrentReport));

                showAlert("Error: Failed to upload report!\n " + trello.ErrorMessage);
                if (trello.UploadException != null)
                    Debug.LogException(trello.UploadException);
                else
                    Debug.LogError(trello.ErrorMessage);

            }
            else
            {
                showAlert("Feedback submitted successfully!");
            }

            submitting = false;
            initCurrentReport();
        }

        /// <summary>
        /// Attaches files on current report to card
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        IEnumerator attachFilesAsync(string cardID)
        {
            for (int i = 0; i < CurrentReport.Attachments.Count; i++)
            {
                FileAttachment attachment = CurrentReport.Attachments[i];
                yield return trello.AddAttachmentAsync(cardID, attachment.Data, null, attachment.Name, null);

                if(trello.UploadError) // failed to add attachment
                {
                    showAlert("Error: Failed to attach file to report!\n" + trello.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Saves the report in a local directory
        /// </summary>
        private string writeLocal(Report report)
        {
            // create the report directory
            string feedbackDirectory = Application.persistentDataPath + "/feedback-" + DateTime.Now.ToString("MMddyyyy-HHmmss");
            Directory.CreateDirectory(feedbackDirectory);

            // save the report
            File.WriteAllText(feedbackDirectory + "/report.txt", report.GetLocalFileText());

            // save screenshot
            File.WriteAllBytes(feedbackDirectory + "/screenshot.png", report.Screenshot);

            // save attachments
            for (int i = 0; i < report.Attachments.Count; i++)
            {
                FileAttachment attachment = report.Attachments[i];
                File.WriteAllBytes(feedbackDirectory + "/" + attachment.Name, attachment.Data);
            }

            return feedbackDirectory;
        }

        /// <summary>
        /// Disables all the Selectable elements on the form.
        /// </summary>
        public void DisableForm()
        {
            foreach (Transform child in Form)
            {
                Selectable selectable = child.GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.interactable = false;
                }
            }
        }

        /// <summary>
        /// Enables all the Selectable elements on the form.
        /// </summary>
        public void EnableForm()
        {
            foreach (Transform child in Form)
            {
                Selectable selectable = child.GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.interactable = true;
                }
            }
        }

        /// <summary>
        /// Hides the form, called by the Close button.
        /// </summary>
        public void Hide()
        {
            // don't do anything if the form is already hidden
            if (!Form.gameObject.activeInHierarchy)
                return;

            // hide form
            Form.gameObject.SetActive(false);

            // delete temporary screenshot
            if (!Config.StoreLocal && IncludeScreenshot
                && File.Exists(screenshotPath))
                File.Delete(screenshotPath);
            screenshotPath = string.Empty;

            // clear screenshot coroutine
            ssCoroutine = null;

            // call OnFormClosed
            OnFormClosed.Invoke();
        }

        /// <param name="preserveState"></param>
        private void releaseMouse()
        {
            // show mouse
            initCursorVisible = Cursor.visible;
            initCursorLockMode = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void hideMouse()
        {
            Cursor.visible = initCursorVisible;
            Cursor.lockState = initCursorLockMode;
        }

        private IEnumerator screenshotAndOpenForm()
        {
            if (IncludeScreenshot)
            {
                // take screenshot before showing the form
                string filename = "debug-" + DateTime.Now.ToString("MMddyyyy-HHmmss") + ".png"; 
                screenshotPath = Path.Combine(Application.persistentDataPath, filename);

#if UNITY_2017 && UNITY_IOS
                ScreenCapture.CaptureScreenshot(filename);
#elif UNITY_2017
                ScreenCapture.CaptureScreenshot(screenshotPath);
#elif UNITY_IOS
                Application.CaptureScreenshot(filename);
#else
                Application.CaptureScreenshot(screenshotPath);
#endif

                // wait to confirm that screenshot has been taken before moving on
                // (so we don't get the form in the screenshot)
                while (!File.Exists(screenshotPath))
                {
                    yield return null;
                }

                // add binary data to report
                CurrentReport.Screenshot = File.ReadAllBytes(screenshotPath);
            }

            releaseMouse();

            // show form
            EnableForm();
            Form.gameObject.SetActive(true);

            // call OnFormOpened
            OnFormOpened.Invoke();
        }
    }
}
