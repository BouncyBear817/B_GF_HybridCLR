// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/31 15:10:26
//  * Description:
//  * Modify Record:
//  *************************************************************/

using DG.DOTweenEditor;
using GameMain.Builtin;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameMain.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DOTweenSequence))]
    public class DOTweenSequenceInspector : UnityEditor.Editor
    {
        private SerializedProperty mSequenceAnimations;
        private ReorderableList mSequenceList;

        private void OnEnable()
        {
            mSequenceAnimations = serializedObject.FindProperty("mSequenceAnimations");
            mSequenceList = new ReorderableList(serializedObject, mSequenceAnimations)
            {
                drawElementCallback = OnDrawElementCallback,
                drawHeaderCallback = OnDrawHeaderCallback,
                elementHeightCallback = OnElementHeightCallback
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var sequence = target as DOTweenSequence;
            if (sequence == null)
                return;

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Play"))
                    {
                        if (DOTweenEditorPreview.isPreviewing)
                        {
                            DOTweenEditorPreview.Stop(true);
                            sequence.DoKill();
                        }

                        DOTweenEditorPreview.PrepareTweenForPreview(sequence.DOPlay());
                        DOTweenEditorPreview.Start();
                    }

                    if (GUILayout.Button("Reverse Play"))
                    {
                        if (DOTweenEditorPreview.isPreviewing)
                        {
                            DOTweenEditorPreview.Stop(true);
                            sequence.DoKill();
                        }

                        DOTweenEditorPreview.PrepareTweenForPreview(sequence.DOReversePlay());
                        DOTweenEditorPreview.Start();
                    }

                    if (GUILayout.Button("Reset"))
                    {
                        DOTweenEditorPreview.Stop(true);
                        sequence.DoKill();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            mSequenceList.DoLayoutList();

            Helper.DrawPropertyField(serializedObject.FindProperty("mPlayOnAwake"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mDelay"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mEase"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mLoops"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mLoopType"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mUpdateType"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mOnPlay"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mOnUpdate"));
            Helper.DrawPropertyField(serializedObject.FindProperty("mOnComplete"));

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = mSequenceAnimations.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, true);
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Sequence Animations");
        }

        private float OnElementHeightCallback(int index)
        {
            var item = mSequenceAnimations.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(item);
        }
    }
}