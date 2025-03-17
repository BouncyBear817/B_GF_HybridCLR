using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public class ToolsMenuExtension
    {
        private static Dictionary<ToolsMenuAttribute, object> sToolsMenuMap = new Dictionary<ToolsMenuAttribute, object>();

        public static void ScanEditorToolbarMenu()
        {
            sToolsMenuMap.Clear();

            var editorDll = Utility.Assembly.GetAssemblies().First(dll => string.Compare(dll.GetName().Name, "Assembly-CSharp-Editor", StringComparison.Ordinal) == 0);
            var allToolsTypes = editorDll.GetTypes();
            foreach (var toolsType in allToolsTypes)
            {
                var attribute = toolsType.GetCustomAttribute<ToolsMenuClassAttribute>();
                if (toolsType.IsClass && !toolsType.IsAbstract && attribute != null)
                {
                    sToolsMenuMap.Add(attribute, toolsType);
                }
                else
                {
                    var methodInfos = toolsType.GetMethods();
                    foreach (var methodInfo in methodInfos)
                    {
                        var methodAttribute = methodInfo.GetCustomAttribute<ToolsMenuMethodAttribute>();
                        if (methodAttribute != null)
                        {
                            sToolsMenuMap.Add(methodAttribute, methodInfo);
                        }
                    }
                }
            }

            sToolsMenuMap = sToolsMenuMap.OrderBy(kvp => kvp.Key.Priority).ThenBy(kvp => kvp.Key.SubPriority).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static void DrawToolsDropdownMenu()
        {
            var popMenu = new GenericMenu();
            foreach (var (attribute, value) in sToolsMenuMap)
            {
                popMenu.AddItem(new GUIContent(attribute.ItemName), false, () => { ClickToolsSubmenu(attribute, value); });
            }

            popMenu.ShowAsContext();
        }

        private static void ClickToolsSubmenu(ToolsMenuAttribute toolsMenuAttribute, object value)
        {
            var type = toolsMenuAttribute.GetType();
            if (type == typeof(ToolsMenuMethodAttribute))
            {
                var methodInfo = value as MethodInfo;
                if (methodInfo != null)
                {
                    var action = Delegate.CreateDelegate(typeof(Action), methodInfo) as Action;
                    action?.Invoke();
                }
            }
            else if (type == typeof(ToolsMenuClassAttribute))
            {
                var windows = EditorWindow.GetWindow(value as Type);
                var classAttribute = toolsMenuAttribute as ToolsMenuClassAttribute;
                if (classAttribute != null && classAttribute.IsUtility)
                {
                    windows.ShowUtility();
                }
                else
                {
                    windows.Show();
                }
            }
        }
    }
}