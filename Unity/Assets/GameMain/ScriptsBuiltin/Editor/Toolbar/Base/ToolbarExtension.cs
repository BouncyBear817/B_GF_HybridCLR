using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    [InitializeOnLoad]
    public static class ToolbarExtension
    {
        private static int sToolCount;
        private static GUIStyle sCommandStyle;

        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        static ToolbarExtension()
        {
            var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            var fieldName = "k_ToolCount";
#else
            var fieldName = "s_ShownToolIcons";
#endif
            var toolIcons = toolbarType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_3_OR_NEWER
            sToolCount = toolIcons != null ? (int)toolIcons.GetValue(null) : 8;
#elif UNITY_2019_1_OR_NEWER
            sToolCount = toolIcons != null ? (int)toolIcons.GetValue(null) : 7;
#elif UNITY_2018_1_OR_NEWER
            sToolCount = toolIcons != null ? ((Array)toolIcons.GetValue(null)).Length : 6;
#else
            sToolCount = toolIcons != null ? ((Array)toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.OnToolbarGUI = OnGUI;
            ToolbarCallback.OnToolbarGUILeft = OnGUILeft;
            ToolbarCallback.OnToolbarGUIRight = OnGUIRight;
        }

#if UNITY_2019_3_OR_NEWER
        private const float Space = 8;
#else
		private const float Space = 10;
#endif
        private const float LargeSpace = 20;
        private const float ButtonWidth = 32;
        private const float DropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
        private const float PlayPauseStopWidth = 140;
#else
		private const float PlayPauseStopWidth = 100;
#endif

        private static void OnGUI()
        {
            // Create two containers, left and right
            // Screen is whole toolbar

            if (sCommandStyle == null)
            {
                sCommandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - PlayPauseStopWidth) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += Space; // Spacing left
            leftRect.xMin += ButtonWidth * sToolCount; // Tool buttons
#if UNITY_2019_3_OR_NEWER
            leftRect.xMin += Space; // Spacing between tools and pivot
#else
			leftRect.xMin += largeSpace; // Spacing between tools and pivot
#endif
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += sCommandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= Space; // Spacing right
            rightRect.xMax -= DropdownWidth; // Layout
            rightRect.xMax -= Space; // Spacing between layout and layers
            rightRect.xMax -= DropdownWidth; // Layers
#if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= Space; // Spacing between layers and account
#else
			rightRect.xMax -= largeSpace; // Spacing between layers and account
#endif
            rightRect.xMax -= DropdownWidth; // Account
            rightRect.xMax -= Space; // Spacing between account and cloud
            rightRect.xMax -= ButtonWidth; // Cloud
            rightRect.xMax -= Space; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab

            // Add spacing around existing controls
            leftRect.xMin += Space;
            leftRect.xMax -= Space;
            rightRect.xMin += Space;
            rightRect.xMax -= Space;

            // Add top and bottom margins
#if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
#else
			leftRect.y = 5;
			leftRect.height = 24;
			rightRect.y = 5;
			rightRect.height = 24;
#endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private static void OnGUILeft()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in LeftToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }

        private static void OnGUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in RightToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }
    }
}