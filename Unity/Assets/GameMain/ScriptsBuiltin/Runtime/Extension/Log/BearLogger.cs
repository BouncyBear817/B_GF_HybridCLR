// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/3 16:42:19
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public static class BearLogger
    {
        private const int STACK_TRACE_SKIP_FRAMES = 3;
        private static readonly StringBuilder mStringBuilder = new();

        public static void ColorInfo(Color color, string message)
        {
            var lines = message.Split('\n');
            var trackBack = message.Replace(lines[0], "");
            Log.Info($"<color=#{GetColorString(color)}>color : '{color}' {lines[0]}</color>{trackBack}");
        }

        public static void Debug(string message)
        {
            OutLog(GameFrameworkLogLevel.Debug, message);
        }

        public static void Info(string message)
        {
            OutLog(GameFrameworkLogLevel.Info, message);
        }

        public static void Warning(string message)
        {
            OutLog(GameFrameworkLogLevel.Warning, message);
        }

        public static void Error(string message)
        {
            OutLog(GameFrameworkLogLevel.Error, message);
        }

        public static void Fatal(string message)
        {
            OutLog(GameFrameworkLogLevel.Fatal, message);
        }

        private static void OutLog(GameFrameworkLogLevel logLevel, string message)
        {
            var infoBuilder = GetStringByColor(logLevel, message);
            if (logLevel == GameFrameworkLogLevel.Warning || logLevel == GameFrameworkLogLevel.Error || logLevel == GameFrameworkLogLevel.Fatal)
            {
                var stackFrames = new StackTrace().GetFrames();
                if (stackFrames != null)
                {
                    infoBuilder.Append("\n");
                    foreach (var frame in stackFrames)
                    {
                        var declaringType = frame.GetMethod().DeclaringType;
                        if (declaringType != null)
                        {
                            var declaringTypeName = declaringType.FullName;
                            var methodName = frame.GetMethod().Name;
                            infoBuilder.Append($"[{declaringTypeName}::{methodName}\n]");
                        }
                    }
                }
            }

            message = infoBuilder.ToString();

#if                                                                                              UNITY_EDITOR
            var stackTrace = new StackTrace(STACK_TRACE_SKIP_FRAMES, true);
            var stackTrackLines = stackTrace.ToString().Split('\n');
            var filteredStackTraceLines = new string[stackTrackLines.Length - STACK_TRACE_SKIP_FRAMES];
            for (var i = 0; i < filteredStackTraceLines.Length; i++)
            {
                filteredStackTraceLines[i] = stackTrackLines[i + STACK_TRACE_SKIP_FRAMES].Trim();
            }

            var filteredStackTrace = string.Join("\n", filteredStackTraceLines);
            message = message + "\n" + filteredStackTrace;
#endif

            if (logLevel == GameFrameworkLogLevel.Debug || logLevel == GameFrameworkLogLevel.Info)
            {
                UnityEngine.Debug.Log(message);
            }
            else if (logLevel == GameFrameworkLogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(message);
            }
            else if (logLevel == GameFrameworkLogLevel.Error || logLevel == GameFrameworkLogLevel.Fatal)
            {
                UnityEngine.Debug.LogError(message);
            }
        }

        private static StringBuilder GetStringByColor(GameFrameworkLogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case GameFrameworkLogLevel.Debug:
                    SetStringByColor(Color.gray, message, "DEBUG");
                    break;
                case GameFrameworkLogLevel.Info:
                    SetStringByColor(Color.white, message, "INFO");
                    break;
                case GameFrameworkLogLevel.Warning:
                    SetStringByColor(Color.yellow, message, "WARNING");
                    break;
                case GameFrameworkLogLevel.Error:
                    SetStringByColor(Color.red, message, "ERROR");
                    break;
                case GameFrameworkLogLevel.Fatal:
                    SetStringByColor(Color.magenta, message, "FATAL");
                    break;
                default:
                    mStringBuilder.Clear();
                    break;
            }

            return mStringBuilder;

            void SetStringByColor(Color _color, string _message, string level)
            {
                mStringBuilder.Clear();
                var messageSplits = _message.Split('\n', 2);
                _message = messageSplits[0].Trim();
                string logString = messageSplits.Length > 1 ? $"<color=#1E90FF>{messageSplits[1].Trim()}</color>" : "";
                mStringBuilder.Append($"<color=#1E90FF><b>[BEAR] ► </b></color><color=#{GetColorString(_color)}><b>[{level}] ► </b> {_message}</color>{logString}");
            }
        }

        private static string GetColorString(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }


        #region 解决日志双击溯源问题

#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            var stackTrace = GetStackTrace();
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(instanceId);
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("[BEAR]"))
            {
                var matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                while (matches.Success)
                {
                    var pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("BearLogger.cs") && !pathLine.Contains("BearLogHelper.cs") && !pathLine.Contains("GameFrameworkLog.cs") && !pathLine.Contains("Log.cs"))
                    {
                        var lineNumber = GetLineNumber(pathLine);
                        var fullPath = GetFullPath(pathLine);
                        if ((!pathLine.Contains(assetPath) || lineNumber != line) && !assetPath.Contains("BearLogger.cs") && !assetPath.Contains("BearLogHelper.cs") &&
                            !assetPath.Contains("GameFrameworkLog.cs") && !assetPath.Contains("Log.cs"))
                        {
                            lineNumber = line;
                            fullPath = GetFullPath(assetPath);
                        }

                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, lineNumber);
                        break;
                    }

                    matches = matches.NextMatch();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前日志窗口选中的日志的堆栈信息
        /// </summary>
        /// <returns>日志的堆栈信息</returns>
        private static string GetStackTrace()
        {
            var consoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var consoleInstance = fieldInfo.GetValue(null);
                if (consoleInstance != null && UnityEditor.EditorWindow.focusedWindow == (UnityEditor.EditorWindow)consoleInstance)
                {
                    fieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo != null)
                    {
                        return fieldInfo.GetValue(consoleInstance).ToString();
                    }
                }
            }

            return null;
        }

        private static string GetFullPath(string assetPath)
        {
            string path;
            if (assetPath.Contains(":"))
            {
                var splitIndex = assetPath.LastIndexOf(':');
                path = assetPath.Substring(0, splitIndex);
            }
            else
            {
                path = assetPath;
            }

            var fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal));
            return (fullPath + path).Replace('/', '\\');
        }

        private static int GetLineNumber(string assetPath)
        {
            var splitIndex = assetPath.LastIndexOf(':');
            return Convert.ToInt32(assetPath.Substring(splitIndex + 1));
        }
#endif

        #endregion
    }
}