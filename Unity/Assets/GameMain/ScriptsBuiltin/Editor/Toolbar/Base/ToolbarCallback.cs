using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

#else
using UnityEngine.Experimental.UIElements;
#endif

namespace GameMain.Editor
{
    public static class ToolbarCallback
    {
        static Type sToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        static Type sGUIViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");

#if UNITY_2020_1_OR_NEWER
        static Type sIWindowBackendType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");
        static PropertyInfo sWindowBackend = sGUIViewType.GetProperty("windowBackend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static PropertyInfo sViewVisualTree = sIWindowBackendType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
         static PropertyInfo sViewVisualTree = sGUIViewType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

        static FieldInfo sImguiContainerOnGUI = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static ScriptableObject sCurrentToolbar;

        // Callback for toolbar OnGUI method.
        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (sCurrentToolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(sToolbarType);
                sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (sCurrentToolbar != null)
                {
#if UNITY_2021_1_OR_NEWER
                    var root = sCurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (root != null)
                    {
                        var rawRoot = root.GetValue(sCurrentToolbar);
                        var visualElement = rawRoot as VisualElement;
                        RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                        RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                        void RegisterCallback(string root, Action cb)
                        {
                            var toolbarZone = visualElement.Q(root);

                            var parent = new VisualElement()
                            {
                                style =
                                {
                                    flexGrow = 1,
                                    flexDirection = FlexDirection.Row,
                                }
                            };
                            var container = new IMGUIContainer();
                            container.style.flexGrow = 1;
                            container.onGUIHandler += () => { cb?.Invoke(); };
                            parent.Add(container);
                            toolbarZone.Add(parent);
                        }
                    }

#else
#if UNITY_2020_1_OR_NEWER
                    var windowBackend = sWindowBackend.GetValue(sCurrentToolbar);
                    var visualTree = sViewVisualTree.GetValue(windowBackend, null) as VisualElement;
#else
                     var visualTree = sViewVisualTree.GetValue(sCurrentToolbar, null) as VisualElement;
#endif
                    if (visualTree != null)
                    {
                        // Get first child which 'happens' to be toolbar IMGUIContainer
                        var container = visualTree[0] as IMGUIContainer;

                        // (Re)attach handler
                        var handler = sImguiContainerOnGUI.GetValue(container) as Action;
                        handler -= OnGUI;
                        handler += OnGUI;
                        sImguiContainerOnGUI.SetValue(container, handler);
                    }


#endif
                }
            }
        }

        static void OnGUI()
        {
            OnToolbarGUI?.Invoke();
        }
    }
}