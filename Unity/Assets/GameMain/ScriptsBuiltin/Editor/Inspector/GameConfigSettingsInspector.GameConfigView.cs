// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/09 10:10:36
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameMain.Builtin;
using GameMain.Hotfix;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    /// <summary>
    /// 游戏配置类型
    /// </summary>
    [Flags]
    public enum GameConfigType
    {
        /// <summary>
        /// 数据表
        /// </summary>
        DataTable = 1,

        /// <summary>
        /// 配置表
        /// </summary>
        Config = 2,

        /// <summary>
        /// 本地化语言表
        /// </summary>
        Localization
    }

    public partial class GameConfigSettingsInspector
    {
        public class ItemData
        {
            public bool IsOn;
            public string Name;

            public ItemData(bool isOn, string name)
            {
                IsOn = isOn;
                Name = name;
            }
        }

        public class GameConfigView
        {
            private GameConfigSettings mGameConfigSettings;
            private bool mFoldout;
            private Vector2 mScrollPosition;
            private string mNewConfigName;

            private string mExcelDir;

            public GameConfigType ConfigType;
            public List<ItemData> ItemDatas;
            public string ConfigPath;

            public GameConfigView(GameConfigSettings gameConfigSettings, GameConfigType configType)
            {
                mGameConfigSettings = gameConfigSettings;
                ConfigType = configType;

                mFoldout = true;

                ItemDatas = new List<ItemData>();
                ConfigPath = GameConfigGenerator.GetGameConfigPrePath(configType);

                mExcelDir = GameConfigGenerator.GetGameConfigExcelPrePath(configType);
            }

            private string[] GetGameConfigList()
            {
                switch (ConfigType)
                {
                    case GameConfigType.DataTable:
                        return GameConfigSettings.GetInstance().DataTables;
                    case GameConfigType.Config:
                        return GameConfigSettings.GetInstance().Configs;
                    case GameConfigType.Localization:
                        return GameConfigSettings.GetInstance().Localizations;
                }

                return null;
            }

            public void Reload()
            {
                if (!Directory.Exists(ConfigPath) || mGameConfigSettings == null)
                {
                    return;
                }

                ItemDatas.Clear();

                var fullPathList = GameConfigGenerator.GetAllGameConfigExcelFullPathList(ConfigType);
                var configArray = GetGameConfigList();
                if (configArray != null)
                {
                    foreach (var path in fullPathList)
                    {
                        var fileName = GameConfigGenerator.GetGameConfigExcelRelativeFileName(ConfigType, path);
                        var isOn = ArrayUtility.Contains(configArray, fileName);
                        ItemDatas.Add(new ItemData(isOn, fileName));
                    }
                }
            }

            public string[] GetSelectedGameConfig()
            {
                var selectedGameConfigs = ItemDatas.Where(data => data.IsOn).ToArray();
                var result = new string[selectedGameConfigs.Length];
                for (var i = 0; i < selectedGameConfigs.Length; i++)
                {
                    result[i] = selectedGameConfigs[i].Name;
                }

                return result;
            }

            private void SelectAllGameConfig(bool isOn)
            {
                foreach (var itemData in ItemDatas)
                {
                    itemData.IsOn = isOn;
                }
            }

            public bool DrawPanel(GUILayoutOption option)
            {
                var dataChanged = false;
                mFoldout = EditorGUILayout.Foldout(mFoldout, ConfigType.ToString());
                if (mFoldout)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition, "box", GUILayout.MaxHeight(100));
                        {
                            EditorGUI.BeginChangeCheck();
                            for (var i = 0; i < ItemDatas.Count; i++)
                            {
                                if (i % ONE_LINE_SHOW_COUNT == 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                }

                                var itemData = ItemDatas[i];
                                itemData.IsOn = EditorGUILayout.ToggleLeft(itemData.Name, itemData.IsOn, option);
                                if (i % ONE_LINE_SHOW_COUNT == ONE_LINE_SHOW_COUNT - 1)
                                {
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                            if (EditorGUI.EndChangeCheck())
                            {
                                dataChanged = true;
                            }

                            if (ItemDatas.Count % ONE_LINE_SHOW_COUNT != 0)
                            {
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndScrollView();

                        EditorGUILayout.BeginHorizontal("box");
                        {
                            if (GUILayout.Button("All", GUILayout.Width(50)))
                            {
                                SelectAllGameConfig(true);
                                dataChanged = true;
                            }

                            if (GUILayout.Button("None", GUILayout.Width(50)))
                            {
                                SelectAllGameConfig(false);
                                dataChanged = true;
                            }

                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Reveal", GUILayout.Width(70)))
                            {
                                EditorUtility.RevealInFinder(ConfigPath);
                                GUIUtility.ExitGUI();
                            }

                            if (GUILayout.Button("Generate", GUILayout.Width(70)))
                            {
                                Generate();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (ConfigType == GameConfigType.DataTable || ConfigType == GameConfigType.Config)
                        {
                            EditorGUILayout.BeginHorizontal("box");
                            {
                                mNewConfigName = EditorGUILayout.TextField(mNewConfigName);
                                if (GUILayout.Button($"New {ConfigType.ToString()}", GUILayout.Width(100)))
                                {
                                    CreateExcel(mNewConfigName);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                return dataChanged;
            }

            private void Generate()
            {
                switch (ConfigType)
                {
                    case GameConfigType.DataTable:
                        GameConfigGenerator.RefreshDataTables(GameConfigGenerator.GetGameConfigExcelRelativeFullPath(ConfigType, GetGameConfigList()));
                        GameConfigGenerator.GenerateGroupEnumScript();
                        GameConfigGenerator.GenerateUIViewScript();
                        break;
                    case GameConfigType.Config:
                        GameConfigGenerator.RefreshConfigs(GameConfigGenerator.GetGameConfigExcelRelativeFullPath(ConfigType, GetGameConfigList()));
                        break;
                    case GameConfigType.Localization:
                        break;
                }
            }

            private void CreateExcel(string newConfigName)
            {
                if (string.IsNullOrEmpty(newConfigName))
                {
                    return;
                }

                var excelPath = PathUtil.GetCombinePath(mExcelDir, newConfigName + EXCEL_EXTENSION);
                if (File.Exists(excelPath))
                {
                    Debug.LogWarning($"Create Excel Failed. Excel file already exists : {excelPath}.");
                    return;
                }

                if (ConfigType == GameConfigType.DataTable)
                {
                    if (GameConfigGenerator.CreateDataTableExcel(excelPath))
                    {
                        Reload();
                        EditorUtility.RevealInFinder(excelPath);
                        GUIUtility.ExitGUI();
                    }
                }
                else if (ConfigType == GameConfigType.Config)
                {
                    if (GameConfigGenerator.CreateConfigExcel(excelPath))
                    {
                        Reload();
                        EditorUtility.RevealInFinder(excelPath);
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }
    }
}