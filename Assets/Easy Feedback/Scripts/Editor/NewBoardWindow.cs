using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EasyFeedback.APIs;
using UnityEngine.Networking;

namespace EasyFeedback.Editor
{
    public class NewBoardWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "New Feedback Board";
        private const int WIDTH = 312;
        private const int MAX_WIDTH = 500;
        private const int HEIGHT = 46;


        private Trello trello;
        private string boardName = "My Feedback Board";
        private ConfigWindow configWindow;

        /// <summary>
        /// Called when the window is shown by the ConfigWindow
        /// </summary>
        /// <param name="configWindow"></param>
        public void SetConfigWindow(ConfigWindow configWindow)
        {
            this.configWindow = configWindow;
        }

        public static NewBoardWindow GetWindow(ConfigWindow configWindow)
        {
            NewBoardWindow window = GetWindow<NewBoardWindow>(true, WINDOW_TITLE);
            window.SetConfigWindow(configWindow);

            // set window size
            window.minSize = new Vector2(WIDTH, HEIGHT);
            window.maxSize = window.minSize;

            return window;
        }

        private void OnEnable()
        {
            if (trello == null)
            {
                // init trello API handler
                EFConfig config = AssetDatabase.LoadAssetAtPath<EFConfig>(ConfigWindow.CONFIG_ASSET_LOCATION);
                trello = new Trello(config.Token);
            }
        }

        private void OnGUI()
        {
            if (trello == null) return;

            boardName = EditorGUILayout.TextField("Board Name", boardName);

            if (GUILayout.Button("Create Board"))
            {
                // add board
                SetupBoard(boardName);

                if (EditorUtility.DisplayDialog("Board created!", "The board " + boardName + " has been successfully created!", "Ok"))
                {
                    // refresh boards
                    if (configWindow)
                        configWindow.RefreshBoardsList();

                    // close self
                    Close();
                }

            }
        }

        /// <summary>
        /// Clones the default feedback board
        /// </summary>
        /// <param name="boardName"></param>
        /// <returns></returns>
        public void SetupBoard(string boardName)
        {
            trello.AddBoard(boardName, true, true, null, null, APIs.Trello.TEMPLATE_BOARD_ID);
        }
    }
}