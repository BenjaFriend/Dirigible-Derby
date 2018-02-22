using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyFeedback.APIs;
using System.Linq;
using System;

namespace EasyFeedback.Editor
{
    public class ConfigWindow : EditorWindow
    {
        public const string CONFIG_ASSET_LOCATION = "Assets/Easy Feedback/EasyFeedbackConfig.asset";

        private const string WINDOW_TITLE = "Easy Feedback Configuration";
        private const int WIDTH = 312;
        private const int HEIGHT = 132;

        private EFConfig config;
        private Trello trello;
        private string token;
        private string lastValidToken;

        /// <summary>
        /// Cache of Trello boards for this account
        /// </summary>
        private Board[] boards;

        /// <summary>
        /// The index of the currently selected board in the boards list
        /// </summary>
        private int selectedBoard;

        /// <summary>
        /// Whether a refresh of the boards cache was requested
        /// </summary>
        private bool boardsRefreshRequested;

        private bool subscribed;

        /// <summary>
        /// Creates a new instance of the window (if there isn't one already)
        /// and displays it.
        /// </summary>
        [MenuItem("Edit/Project Settings/Easy Feedback Form")]
        public static void Init()
        {
            // get existing window or make a new one
            ConfigWindow window = GetWindow<ConfigWindow>(true, WINDOW_TITLE);
            
            // set window size
            window.maxSize = new Vector2(WIDTH, HEIGHT);
            window.minSize = window.maxSize;
        }

        private void Update()
        {
            // wait until authenticated
            if (!config || string.IsNullOrEmpty(config.Token)) return;

            if (!Application.isPlaying)
                updateEditorOnly();
        }

        public void updateEditorOnly()
        {
            // init Trello API handler if we need to
            if (trello == null && config != null)
            {
                initAPIHandler();
            }

            // block until API handler is initialized
            // it can still be null after initAPIHandler if the token was not valid
            if (trello == null) return;

            // refresh boards if we need to
            if (boardsRefreshRequested || boards == null)
            {
                RefreshBoardsList();
                refreshBoardInfo();
                boardsRefreshRequested = false;
            }

            // refresh board info if we need to
            if (boards.Length > 0 && config.Board.Id != boards[selectedBoard].id)
            {
                refreshBoardInfo();
            }
        }

        /// <summary>
        /// Initializes a new instance of the Trello API handler
        /// </summary>
        private void initAPIHandler()
        {
            if (Trello.IsValidToken(config.Token))
            {
                trello = new Trello(config.Token);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Token", "Invalid Trello API credentials. Please re-enter your token.", "OK");
                // remove bad token to force reauth
                logOut();
            }
        }

        /// <summary>
        /// Refreshes the boards list
        /// </summary>
        public void RefreshBoardsList()
        {
            // show progress bar
            refreshProgressBar(0f);

            // get all boards
            Board[] allBoards = trello.GetBoards();

            // select only feedback boards
            boards = allBoards.Where(b => isFeedbackBoard(b)).ToArray();

            // set selected index to current board from config
            bool foundCurrentBoard = false;
            for (int i = 0; i < boards.Length; i++)
            {
                if (boards[i].id == config.Board.Id)
                {
                    foundCurrentBoard = true;
                    selectedBoard = i;
                }
            }

            // reset selected board index if it no longer exists on the list
            if (!foundCurrentBoard)
                selectedBoard = 0;

            // update progress bars
            refreshProgressBar(0.8f);

            // update subscribed check for current board
            if (boards.Length > 0)
                updateSubscribed();

            // clear progress bar
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Checks if the user is subscribed to the current board
        /// </summary>
        private void updateSubscribed()
        {
            subscribed = trello.GetSubscribed(boards[selectedBoard].id);
        }

        /// <summary>
        /// Checks if a board has all of the required feedback components
        /// </summary>
        /// <returns></returns>
        private bool isFeedbackBoard(Board board)
        {
            // check if the board is closed
            if (board.closed) return false;

            // check that there are category lists
            if (!boardHasCategoryLists(board)) return false;

            // passed checks
            return true;
        }

        /// <summary>
        /// Check if a board has category lists (part of isFeedbackBoard check)
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private bool boardHasCategoryLists(Board board)
        {
            List[] lists = trello.GetLists(board.id);
            for (int i = 0; i < lists.Length; i++)
            {
                if (lists[i].name.EndsWith(Trello.CATEGORY_TAG)) return true;
            }

            return false;
        }

        private void refreshBoardInfo()
        {
            // don't refresh current board if there aren't any feedback boards on the account
            if (boards.Length == 0)
                return;

            setBoardProgressBar(boards[selectedBoard].name, "Setting board info", 0f);

            config.Board.Id = boards[selectedBoard].id;

            setBoardProgressBar(boards[selectedBoard].name, "Getting lists and categories", 0.3f);

            // update lists and categories
            updateLists();

            setBoardProgressBar(boards[selectedBoard].name, "Getting labels", 0.6f);

            // set board priority labels
            config.Board.Labels = trello.GetLabels(boards[selectedBoard].id);

            // sort labels by id
            config.Board.Labels = config.Board.Labels.OrderBy(l => l.id).ToArray();

            setBoardProgressBar(boards[selectedBoard].name, "Checking subscribed state", 0.9f);

            // update subscribed checkbox
            updateSubscribed();

            EditorUtility.ClearProgressBar();

            // save asset
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Gets lists from the Trello API, then updates categories
        /// </summary>
        private void updateLists()
        {
            List[] lists = trello.GetLists(boards[selectedBoard].id);

            config.Board.ListIds = new string[lists.Length];
            config.Board.ListNames = new string[lists.Length];

            for (int i = 0; i < lists.Length; i++)
            {
                config.Board.ListIds[i] = lists[i].id;
                config.Board.ListNames[i] = lists[i].name;
            }

            EditorUtility.SetDirty(config);

            updateCategories(lists);
        }

        /// <summary>
        /// Checks for Easy Feedback (EF) lists on the board
        /// </summary>
        private void updateCategories(List[] lists)
        {

            IEnumerable<List> efLists = lists.Where(l => l.name.EndsWith(Trello.CATEGORY_TAG));

            // get shortnames
            config.Board.CategoryNames = efLists.Select(
                l => l.name.Substring(0, l.name.Length - Trello.CATEGORY_TAG.Length)).ToArray();

            // get ids
            config.Board.CategoryIds = efLists.Select(l => l.id).ToArray();

            EditorUtility.SetDirty(config);
        }

        /// <summary>
        /// Displays or updates the progress bar for refreshing the boards list
        /// </summary>
        /// <param name="progress"></param>
        private void refreshProgressBar(float progress)
        {
            EditorUtility.DisplayProgressBar("Loading Boards", "Loading list of boards from Trello", progress);
        }

        /// <summary>
        /// Displays or updates the progress bar for setting the feedback board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="message"></param>
        /// <param name="progress"></param>
        private void setBoardProgressBar(string board, string message, float progress)
        {
            EditorUtility.DisplayProgressBar("Loading board: " + board, message, progress);
        }

        private void OnEnable()
        {
            // attempt to load config if it hasn't already been loaded
            if (!config)
            {
                loadConfigAsset();
            }

        }

        private void OnFocus()
        {
            // (re)load config asset and api handler
            loadConfigAsset();

            // check that Trello key is still valid if it has changed
            if (!string.IsNullOrEmpty(lastValidToken) && lastValidToken != config.Token && tokenIsValid(config.Token))
            {
                // update lastValidToken, reinit api handler
                lastValidToken = config.Token;
                initAPIHandler();
            }
            else if (!string.IsNullOrEmpty(lastValidToken) && lastValidToken != config.Token)
            {
                // log out
                logOut();
            }
        }

        /// <summary>
        /// Attempts to load the TrelloInfo asset from the EasyFeedback directory
        /// </summary>
        /// <returns></returns>
        private void loadConfigAsset()
        {
            config = AssetDatabase.LoadAssetAtPath<EFConfig>(CONFIG_ASSET_LOCATION);
            if (!config) // no config file yet, create one
            {
                createConfigAsset();
            }
        }

        private void OnGUI()
        {
            // wait until config is created to display anything
            if (config == null) return;

            // request authentication if Trello info is null on config asset
            if (string.IsNullOrEmpty(config.Token))
            {
                showAuth();
                resetButton();
            }

            // wait until API handler has been initalized to show Trello controls
            if (trello != null)
            {
                // show log out option
                logOutButton();

                // show new board option
                newBoardButton();

                // show refresh boards button
                refreshBoardsButton();

                // show board controls
                boardControls();
            }

            bool lastSLVal = config.StoreLocal;
            config.StoreLocal = EditorGUILayout.Toggle("Store Locally", config.StoreLocal);

            if(lastSLVal != config.StoreLocal)
                EditorUtility.SetDirty(config);
        }

        /// <summary>
        /// Shows controls for authenticating with Trello
        /// </summary>
        /// <returns></returns>
        private void showAuth()
        {
            // show Trello API token field
            token = EditorGUILayout.TextField("Token", token);

            // show authenticate button
            if (GUILayout.Button("Authenticate With Token") && tokenIsValid(token))
            {
#if UNITY_5
                if (WebWindow.window) // close web window
                {
                    WebWindow.window.Close();
                }
#endif
                // user has authenticated with Trello, update config
                config.StoreLocal = false;
                config.Token = token;
                lastValidToken = token;

                // save config
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                // request a refresh (first load) of account when the api handler is initialized
                boardsRefreshRequested = true;
            }

            // direct user to Trello API auth page if the window isn't already open
            if (!WebWindow.window && GUILayout.Button("Get Trello API Token"))
            {
#if UNITY_5
                // show authorization window
                WebWindow.GetWindow(Trello.AuthURL);
#else
                // prompt user to auth via browser
                Application.OpenURL(Trello.AuthURL);
#endif
            }
        }

        /// <summary>
        /// Shows the "Reset Config" button, and handles clicks
        /// </summary>
        public void resetButton()
        {
            if (GUILayout.Button("Reset to Default")
                && EditorUtility.DisplayDialog("Reset Config", "Are you sure you want to reset the Easy Feedback config file to default values?", "Yes", "Cancel"))
            {
                // replace config with default values
                config.StoreLocal = true;
                config.Token = null;
                config.Board = new FeedbackBoard();

                // save config
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Reset Successful", "The Easy Feedback config file has been reset to default values.", "OK");
            }
        }

        public void createConfigAsset()
        {
            // create the config asset
            config = new EFConfig();
            AssetDatabase.CreateAsset(config, CONFIG_ASSET_LOCATION);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            // update Feedback prefab
            GameObject feedbackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Easy Feedback/Feedback.prefab");
            feedbackPrefab.GetComponent<FeedbackForm>().Config = config;
            EditorUtility.SetDirty(feedbackPrefab);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Checks if the current token is valid, shows a message if invalid
        /// </summary>
        /// <returns></returns>
        private bool tokenIsValid(string token)
        {
            bool valid = Trello.IsValidToken(token);
            if (!valid)
            {
                // show message
                EditorUtility.DisplayDialog("Invalid Token", "The provided token is invalid. Please enter a valid Trello API token.", "OK");

                // highlight token field
                EditorGUI.FocusTextInControl("Token");
            }
            return valid;
        }

        /// <summary>
        /// Displays the "Log Out of Trello" button, and handles clicks
        /// </summary>
        private void logOutButton()
        {
            if (GUILayout.Button("Log Out of Trello"))
            {
                logOut();
            }
        }

        /// <summary>
        /// Logs out of Trello
        /// </summary>
        private void logOut()
        {
            // clear token
            token = string.Empty;
            lastValidToken = null;

            // clear temporary variables
            trello = null;
            boards = null;
            selectedBoard = 0;

            // reset Trello info in config
            // not resetting FeedbackBoard to preserve categories and labels from Trello
            config.Token = null;
            config.StoreLocal = true; // default to store local when not logged in

            // save config
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            // unfocus token text area so it properly clears
            EditorGUIUtility.hotControl = 0;
            EditorGUIUtility.keyboardControl = 0;
        }

        /// <summary>
        /// Displays the "New Board" button, and handles clicks
        /// </summary>
        private void newBoardButton()
        {
            if (GUILayout.Button("New Board"))
            {
                // show new board window
                NewBoardWindow.GetWindow(this);
            }
        }

        /// <summary>
        /// Displays the "Refresh Boards" button, and handles clicks
        /// </summary>
        private void refreshBoardsButton()
        {
            if (GUILayout.Button("Refresh Boards"))
            {
                boardsRefreshRequested = true;
            }
        }

        /// <summary>
        /// Displays controls for managing boards
        /// </summary>
        private void boardControls()
        {
            if (boards != null && boards.Length > 0)
            {
                string[] boardNames = boards.Select((b) => b.name).ToArray();
                selectedBoard = EditorGUILayout.Popup("Feedback Board", selectedBoard, boardNames);

                // show subscribed checkbox underneath boards list
                if (EditorGUILayout.Toggle("Subscribe to board", subscribed) != subscribed)
                {
                    // switch subscribed state
                    setSubscribed(!subscribed); // TODO: this should probably be moved to Update
                }
            }
            else // no boards on this account yet
            {
                GUI.skin.label.wordWrap = true;
                GUILayout.Label("There are no Easy Feeback boards on this account yet!\nClick \"New Board\" above to create one!");
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes a user from the selected board
        /// </summary>
        /// <param name="value">Whether or not the user is subscribed to the board</param>
        private void setSubscribed(bool value)
        {
            trello.PutSubscribed(boards[selectedBoard].id, value);
            updateSubscribed();
        }
    }
}

