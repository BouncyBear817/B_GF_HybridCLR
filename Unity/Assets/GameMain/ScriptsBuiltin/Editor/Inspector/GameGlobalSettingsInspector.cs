// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/18 15:10:44
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameMain.Builtin;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    [CustomEditor(typeof(GameGlobalSettings))]
    public class GameGlobalSettingsInspector : UnityEditor.Editor
    {
        private GameGlobalSettings mGameGlobalSettings;
        private SerializedProperty mServerType;

        private void OnEnable()
        {
            mGameGlobalSettings = target as GameGlobalSettings;

            mServerType = serializedObject.FindProperty("mServerType");

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.TextField("Global Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Script Author", GUILayout.Width(160f));
                    mGameGlobalSettings.ScriptAuthor = EditorGUILayout.TextField(mGameGlobalSettings.ScriptAuthor);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Main Font", GUILayout.Width(160f));
                    mGameGlobalSettings.MainFont = (Font)EditorGUILayout.ObjectField("", mGameGlobalSettings.MainFont, typeof(Font), false);
                }
                EditorGUILayout.EndHorizontal();
                
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}