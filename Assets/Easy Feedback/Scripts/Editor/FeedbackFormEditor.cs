using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace EasyFeedback.Editor
{
    [CustomEditor(typeof(FeedbackForm))]
    public class FeedbackFormEditor : UnityEditor.Editor
    {
        private SerializedProperty Config;

        void Awake()
        {
            Config = serializedObject.FindProperty("Config");
        }

        public override void OnInspectorGUI()
        {
            // show Trello info if setup
            if (Config == null || !Config.objectReferenceValue)
            {
                
                EditorGUILayout.LabelField("Easy Feedback is not yet configured!");
                if (GUILayout.Button("Configure Now"))
                {
                    ConfigWindow.Init();
                }
            }
            else
            {
                EFConfig config = Config.objectReferenceValue as EFConfig;
                if (string.IsNullOrEmpty(config.Token))
                {
                    EditorGUILayout.LabelField("Not authenticated with Trello!");
                    if (GUILayout.Button("Authenticate Now"))
                    {
                        ConfigWindow.Init();
                    }
                }
            }
            
            base.OnInspectorGUI();
        }
    }
}