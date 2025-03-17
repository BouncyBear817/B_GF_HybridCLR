// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/18 14:10:52
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameMain.Builtin;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    [CustomEditor(typeof(GamePathSettings))]
    public class GamePathSettingsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);
            EditorGUILayout.TextField("Game Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mConfigPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mDataTablePath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mLocalizationPath"));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.TextField("Game Config Excel", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mConfigExcelPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mDataTableExcelPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mLocalizationExcelPath"));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.TextField("Data Table Core Excel", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mEntityGroupDataTableExcelPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mSoundGroupDataTableExcelPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mUIGroupDataTableExcelPath"));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.TextField("Data Table Core Group Code", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mDataTableGroupCodePath"));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.TextField("Game Asset", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mEntityPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mFontPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mMusicPath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mScenePath"));
                Helper.DrawPropertyField(serializedObject.FindProperty("mSoundPath"));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.TextField("UI", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                Helper.DrawPropertyField(serializedObject.FindProperty("mUIPath"));
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }
    }
}