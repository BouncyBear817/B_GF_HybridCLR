using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameMain.Editor
{
    [ToolsMenuClass("Other/Find Missing Scripts", null, 40, 40)]
    public class MissingScriptsListWindow : EditorWindow
    {
        private class Info
        {
            public Object Obj;
            public GUIContent Content;
        }

        private static string[] sScriptBelongList = new[]
        {
            "In Open Scenes",
            "On Prefabs"
        };

        private int mSelectedIndex;
        private Vector2 mScroll;
        private List<Info> mEntityList = new List<Info>();
        private GUIStyle mLeftAlignButtonStyle;

        private void OnEnable()
        {
            mSelectedIndex = 0;
            mEntityList.Clear();

            mLeftAlignButtonStyle = EditorStyles.miniButton;
            mLeftAlignButtonStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Find", EditorStyles.miniButton, GUILayout.Width(80f)))
                {
                    if (mSelectedIndex == 0)
                    {
                        FindFromScene();
                    }
                    else if (mSelectedIndex == 1)
                    {
                        FindFromPrefab();
                    }
                }

                mSelectedIndex = EditorGUILayout.Popup(mSelectedIndex, sScriptBelongList, GUILayout.Width(150f));
                // GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            mScroll = EditorGUILayout.BeginScrollView(mScroll, "box");
            {
                foreach (var info in mEntityList)
                {
                    if (GUILayout.Button(info.Content, mLeftAlignButtonStyle, GUILayout.ExpandWidth(false)))
                    {
                        EditorGUIUtility.PingObject(info.Obj);
                        Selection.activeObject = info.Obj;
                    }
                }

                GUILayout.Space(50);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndScrollView();
        }

        private void FindFromScene()
        {
            mEntityList.Clear();

            var scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                foreach (var child in rootObject.GetComponentsInChildren<Transform>())
                {
                    var count = 0;
                    var components = child.GetComponents<Component>();
                    foreach (var com in components)
                    {
                        if (com == null)
                        {
                            count++;
                        }
                    }

                    if (count == 0)
                    {
                        continue;
                    }

                    var parent = child.transform.parent;
                    var info = new Info()
                    {
                        Obj = child,
                        Content = new GUIContent(child.name)
                    };
                    mEntityList.Add(info);

                    while (parent != null)
                    {
                        info.Content.text = $"{parent.name}/{info.Content.text}";
                        parent = parent.parent;
                    }

                    info.Content.text = $"[{count}] {info.Content.text}";

                    mEntityList.Sort(((a, b) => string.Compare(a.Content.text, b.Content.text, StringComparison.Ordinal)));
                }
            }
        }

        private void FindFromPrefab()
        {
            mEntityList.Clear();

            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var assetPath in allAssetPaths)
            {
                if (Path.GetExtension(assetPath) == ".prefab")
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    foreach (var child in go.GetComponentsInChildren<Transform>())
                    {
                        var count = 0;
                        var components = child.GetComponents<Component>();
                        foreach (var com in components)
                        {
                            if (com == null)
                            {
                                count++;
                            }
                        }

                        if (count == 0)
                        {
                            continue;
                        }

                        var parent = child.transform.parent;
                        var info = new Info()
                        {
                            Obj = child,
                            Content = new GUIContent(child.name)
                        };
                        mEntityList.Add(info);

                        while (parent != null)
                        {
                            info.Content.text = $"{parent.name}/{info.Content.text}";
                            parent = parent.parent;
                        }

                        info.Content.text = $"[{count}] {info.Content.text}";

                        mEntityList.Sort(((a, b) => string.Compare(a.Content.text, b.Content.text, StringComparison.Ordinal)));
                    }
                }
            }
        }
    }
}