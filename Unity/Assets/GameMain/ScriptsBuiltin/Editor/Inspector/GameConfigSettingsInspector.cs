// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/09 10:10:36
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameFramework;
using GameMain.Builtin;
using GameMain.Hotfix;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    [CustomEditor(typeof(GameConfigSettings))]
    public partial class GameConfigSettingsInspector : UnityEditor.Editor
    {
        private const int ONE_LINE_SHOW_COUNT = 3;
        private const string EXCEL_EXTENSION = ".xlsx";

        bool mProcedureFoldout = true;
        Vector2 mProcedureScrollPos;
        ItemData[] mProcedures;

        private GameConfigSettings mGameConfigSettings;
        private GameConfigView[] mGameConfigViews;

        private void OnEnable()
        {
            mGameConfigSettings = (GameConfigSettings)target;
            mGameConfigViews = new[]
            {
                new GameConfigView(mGameConfigSettings, GameConfigType.Config),
                new GameConfigView(mGameConfigSettings, GameConfigType.DataTable),
                new GameConfigView(mGameConfigSettings, GameConfigType.Localization)
            };

            ReloadView(mGameConfigSettings);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var itemWidth = GUILayout.Width(Mathf.Max(EditorGUIUtility.currentViewWidth / ONE_LINE_SHOW_COUNT - 20, 100));

            DrawProcedure(itemWidth);

            foreach (var gameConfigView in mGameConfigViews)
            {
                if (gameConfigView.DrawPanel(itemWidth))
                {
                    SaveConfig(mGameConfigSettings);
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal("box");
            {
                if (GUILayout.Button("Reload", GUILayout.Height(30)))
                {
                    ReloadView(mGameConfigSettings);
                }

                if (GUILayout.Button("Save", GUILayout.Height(30)))
                {
                    SaveConfig(mGameConfigSettings);
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void SaveConfig(GameConfigSettings gameConfigSettings)
        {
            foreach (var gameConfigView in mGameConfigViews)
            {
                var type = gameConfigSettings.GetType();
                if (gameConfigView.ConfigType == GameConfigType.Config)
                {
                    var field = type.GetField("mConfigs", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(gameConfigSettings, gameConfigView.GetSelectedGameConfig());
                    }
                }
                else if (gameConfigView.ConfigType == GameConfigType.DataTable)
                {
                    var field = type.GetField("mDataTables", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(gameConfigSettings, gameConfigView.GetSelectedGameConfig());
                    }
                }
                else if (gameConfigView.ConfigType == GameConfigType.Localization)
                {
                    var field = type.GetField("mLocalizations", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(gameConfigSettings, gameConfigView.GetSelectedGameConfig());
                    }
                }
            }

            var selectedProcedure = new List<string>();
            foreach (var procedure in mProcedures)
            {
                if (!procedure.IsOn)
                {
                    continue;
                }
                selectedProcedure.Add(procedure.Name);
            }
            var procedureField = gameConfigSettings.GetType().GetField("mProcedures", BindingFlags.Instance | BindingFlags.NonPublic);
            if (procedureField != null)
            {
                procedureField.SetValue(gameConfigSettings, selectedProcedure.ToArray());
            }

            EditorUtility.SetDirty(gameConfigSettings);
        }

        private void ReloadView(GameConfigSettings gameConfigSettings)
        {
            foreach (var gameConfigView in mGameConfigViews)
            {
                gameConfigView.Reload();
            }

            ReloadProcedures(gameConfigSettings);
        }

        private void DrawProcedure(GUILayoutOption itemWidth)
        {
            mProcedureFoldout = EditorGUILayout.Foldout(mProcedureFoldout, new GUIContent("Procedure"));
            if (mProcedureFoldout)
            {
                EditorGUILayout.BeginVertical();
                {
                    mProcedureScrollPos = EditorGUILayout.BeginScrollView(mProcedureScrollPos, "box", GUILayout.Height(100));
                    {
                        EditorGUI.BeginChangeCheck();
                        for (var i = 0; i < mProcedures.Length; i++)
                        {
                            if (i % ONE_LINE_SHOW_COUNT == 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                            }

                            var itemData = mProcedures[i];
                            itemData.IsOn = EditorGUILayout.ToggleLeft(itemData.Name, itemData.IsOn, itemWidth);
                            if (i % ONE_LINE_SHOW_COUNT == ONE_LINE_SHOW_COUNT - 1)
                            {
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            SaveConfig(mGameConfigSettings);
                        }

                        if (mProcedures.Length % ONE_LINE_SHOW_COUNT != 0)
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void ReloadProcedures(GameConfigSettings gameConfigSettings)
        {
            mProcedures ??= new ItemData[] { };
            ArrayUtility.Clear(ref mProcedures);

            var hotfixDlls = Utility.Assembly.GetAssemblies().Where(dll => HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesIncludePreserved.Contains(dll.GetName().Name)).ToArray();
            foreach (var dll in hotfixDlls)
            {
                var procedureClassArray = dll.GetTypes().Where(type => type.BaseType == typeof(ProcedureBase)).ToArray();
                foreach (var type in procedureClassArray)
                {
                    ArrayUtility.Add(ref mProcedures, new ItemData(gameConfigSettings.Procedures.Contains(type.FullName), type.FullName));
                }
            }
        }
    }
}