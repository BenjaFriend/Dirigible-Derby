using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.Events;
using System.Timers;

namespace EasyFeedback.Editor
{
    /// <summary>
    /// Displays a web browser in editor
    /// </summary>
    public class WebWindow : ScriptableObject
    {
        private const BindingFlags ALL_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        private const string TITLE = "Authorize Trello API";
        private const int WIDTH = 620;
        private const int HEIGHT = 670;

        public static EditorWindow window;
        private static Type webViewEditorWindowTabsType;
        private static MethodInfo executeJavascriptMethod;
        private static FieldInfo initialOpenUrl;
        private static FieldInfo webViewField;

        /// <summary>
        /// Creates a new instance of the WebViewEditorWindow class
        /// </summary>
        public static EditorWindow GetWindow(string sourcesPath)
        {
            // focus existing window
            if (window != null)
            {
                window.Focus();
            }

            // get the WebViewEditorWindow type
            webViewEditorWindowTabsType = getTypeFromAssemblies("WebViewEditorWindowTabs");

            // get the Create method from the WebViewEditorWindow type and make the generic method
            MethodInfo createUtility = webViewEditorWindowTabsType.GetMethod("CreateUtility", ALL_FLAGS | BindingFlags.FlattenHierarchy);
            MethodInfo createUtilityGeneric = createUtility.MakeGenericMethod(webViewEditorWindowTabsType);

            // instantiate the window
            window = (EditorWindow)createUtilityGeneric.Invoke(null, new object[]{ TITLE, sourcesPath, WIDTH, WIDTH, HEIGHT, HEIGHT });

            // get the webview
            webViewField = webViewEditorWindowTabsType.GetField("m_WebView", ALL_FLAGS);

            // get ExecuteJavascript
            Type webViewType = getTypeFromAssemblies("WebView");
            executeJavascriptMethod = webViewType.GetMethod("ExecuteJavascript", ALL_FLAGS);

            // register update
            EditorApplication.update += update;

            return window;
        }

        private static void setBackgroundColor()
        {
            // execute js to set background color if there isn't one
            executeJS("if(getComputedStyle(document.body)['background-color'] === 'rgba(0, 0, 0, 0)'){document.body.style.backgroundColor = 'white';}");
        }

        private static void update()
        {
            if (window != null && webViewField.GetValue(window) != null)
            {
                setBackgroundColor();
            }
            else if(window == null) // window doesn't exist, unregister update
            {
                EditorApplication.update -= update;
            }
        }

        private static void executeJS(string js)
        {
            executeJavascriptMethod.Invoke(webViewField.GetValue(window), new object[] { js });
        }

        /// <summary>
        /// Extracts a type from the application's assemblies
        /// </summary>
        /// <param name="typeName">The name of the type</param>
        /// <returns>The extracted type</returns>
        private static Type getTypeFromAssemblies(string typeName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)
                        || type.Name.Contains('+' + typeName))
                        return type;
                }
            }

            Debug.LogWarning("Could not find type \"" + typeName + "\" in assemblies.");
            return null;
        }
    }
}

