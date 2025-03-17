using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameMain.Builtin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public class HybridCLRExtension
    {
        private const string DISABLE_HYBRIDCLR = "DISABLE_HYBRIDCLR";

        [MenuItem("HybridCLR/Compile Dll And Copy [Generate HotUpdate Dll]", false, 4)]
        public static void CompileTargetDll()
        {
            CompileTargetDll(false);
        }

        public static void RefreshHybridCLREnable()
        {
            var target = GetCurrentNamedBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(target, out var defines);

            if (ArrayUtility.Contains(defines, DISABLE_HYBRIDCLR))
            {
                ArrayUtility.Remove(ref defines, DISABLE_HYBRIDCLR);
                PlayerSettings.SetScriptingDefineSymbols(target, defines);
            }

            RefreshPlayerSettings();
            
            RefreshAssemblyDefinition(false);

            HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable = true;
            HybridCLR.Editor.Settings.HybridCLRSettings.Save();
            EditorUtility.DisplayDialog("HybridCLR", "Switch to updatable mode, HybridCLR hot update enable! Remember to add the hot update DLL resource in the ResourceEditor.", "ok");
        }

        public static void RefreshHybridCLRDisable()
        {
            var target = GetCurrentNamedBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(target, out var defines);

            if (!ArrayUtility.Contains(defines, DISABLE_HYBRIDCLR))
            {
                ArrayUtility.Add(ref defines, DISABLE_HYBRIDCLR);
                PlayerSettings.SetScriptingDefineSymbols(target, defines);
            }

            RefreshPlayerSettings();
            
            RefreshAssemblyDefinition(true);

            HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable = false;
            HybridCLR.Editor.Settings.HybridCLRSettings.Save();
            EditorUtility.DisplayDialog("HybridCLR", "Switch to standalone mode, HybridCLR hot update disabled! Remember to remove the hot update DLL resource in the ResourceEditor.", "ok");
        }

        private static void RefreshPlayerSettings()
        {
#if DISABLE_HYBRIDCLR
            PlayerSettings.gcIncremental = true;
#else
            PlayerSettings.gcIncremental = false;
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_Unity_4_8);
#endif
        }

        private static void RefreshAssemblyDefinition(bool disableHybridCLR)
        {
            var assetParentDir = Directory.GetParent(Application.dataPath)?.FullName;

            var builtinFile = PathUtil.GetCombinePath(assetParentDir, Constant.BuiltinAssembly);
            var textData = File.ReadAllText(builtinFile);
            var jsonData = JsonConvert.DeserializeObject<JObject>(textData);
            var refAssemblies = jsonData["references"] as JArray;
            if (refAssemblies == null)
            {
                return;
            }

            if (refAssemblies.Count > 0)
            {
                var assembly = refAssemblies[0].Value<string>();
                if (assembly.StartsWith("GUID:"))
                {
                    EditorUtility.DisplayDialog("Error", $"解析Assembly Definition '{Constant.BuiltinAssembly}' 失败. 请将Use GUIDs设置为false后重试.", "OK");
                    return;
                }
            }

            if (disableHybridCLR)
            {
                var changed = false;
                for (var i = refAssemblies.Count - 1; i >= 0; i--)
                {
                    if (string.Compare(refAssemblies[i].Value<string>(), "HybridCLR.Runtime", StringComparison.Ordinal) == 0)
                    {
                        refAssemblies.RemoveAt(i);
                        changed = true;
                        break;
                    }
                }

                if (changed)
                {
                    File.WriteAllText(builtinFile, jsonData.ToString(Formatting.Indented));
                    AssetDatabase.Refresh();
                }

                Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", string.Empty);
                Debug.Log("Remove environment variable : UNITY_IL2CPP_PATH");
            }
            else
            {
                var hasValue = false;
                for (var i = refAssemblies.Count - 1; i >= 0; i--)
                {
                    if (string.Compare(refAssemblies[i].Value<string>(), "HybridCLR.Runtime", StringComparison.Ordinal) == 0)
                    {
                        hasValue = true;
                        break;
                    }
                }

                if (!hasValue)
                {
                    refAssemblies.Add("HybridCLR.Runtime");
                    File.WriteAllText(builtinFile, jsonData.ToString(Formatting.Indented));
                    AssetDatabase.Refresh();
                }

                if (Directory.Exists(HybridCLR.Editor.SettingsUtil.LocalIl2CppDir))
                {
                    Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", HybridCLR.Editor.SettingsUtil.LocalIl2CppDir);
                    Debug.Log($"Set environment variable : UNITY_IL2CPP_PATH : {HybridCLR.Editor.SettingsUtil.LocalIl2CppDir}");
                }
            }
        }

        public static void CompileTargetDll(bool includeAotDll)
        {
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            var destDir = PathUtil.GetCombinePath(Application.dataPath, Constant.HotfixDllDir);
            var dllFiles = Directory.GetFiles(destDir, "*.dll.bytes");
            for (var i = dllFiles.Length - 1; i >= 0; i--)
            {
                File.Delete(dllFiles[i]);
            }

            var failureList = CopyHotfixDllsTo(EditorUserBuildSettings.activeBuildTarget, destDir, includeAotDll);
            var content = "Compile dlls and copy to ‘GameMain/HotfixDlls’ success.";
            if (failureList.Length > 0)
            {
                content = "Error! Missing file : \r\n";
                foreach (var failureDll in failureList)
                {
                    content += failureDll + "\r\n";
                }

                EditorUtility.DisplayDialog("Compile And Copy", content, "OK");
            }
        }

        /// <summary>
        /// 把热更新dll拷贝到指定目录
        /// </summary>
        /// <param name="target">平台</param>
        /// <param name="destDir">拷贝到目标目录</param>
        /// <param name="copyAotMeta">是否同时拷贝AOT元数据补充dll</param>
        /// <returns></returns>
        private static string[] CopyHotfixDllsTo(BuildTarget target, string destDir, bool copyAotMeta = true)
        {
            var failureList = new List<string>();
            var hotfixDllDir = HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            foreach (var dll in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
            {
                var dllPath = PathUtil.GetCombinePath(hotfixDllDir, dll);
                if (File.Exists(dllPath))
                {
                    var dllBytesPath = PathUtil.GetCombinePath(destDir, $"{dll}.bytes");
                    File.Copy(dllPath, dllBytesPath, true);
                }
                else
                {
                    failureList.Add(dllPath);
                }
            }

            if (copyAotMeta)
            {
                var failureNames = CopyAotDllsToProject(target);
                failureList.AddRange(failureNames);
            }

            var hotfixListFile = PathUtil.GetCombinePath(Application.dataPath, Constant.HotfixListFile);
            File.WriteAllText(hotfixListFile, JsonConvert.SerializeObject(HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyFilesIncludePreserved), Encoding.UTF8);
            AssetDatabase.Refresh();
            return failureList.ToArray();
        }

        public static string[] CopyAotDllsToProject(BuildTarget target)
        {
            var failureList = new List<string>();
            var aotDllDir = HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            var aotSaveDir = PathUtil.GetCombinePath(Application.dataPath, Constant.AotDlls);
            if (Directory.Exists(aotSaveDir))
            {
                Directory.Delete(aotSaveDir, true);
            }

            Directory.CreateDirectory(aotSaveDir);
            foreach (var aot in HybridCLR.Editor.SettingsUtil.AOTAssemblyNames)
            {
                var aotPath = PathUtil.GetCombinePath(aotDllDir, aot.EndsWith(".dll") ? aot : aot + ".dll");
                if (!File.Exists(aotPath))
                {
                    Debug.LogWarning($"ab中添加AOT补充元数据dll:{aotPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    failureList.Add(aotPath);
                    continue;
                }

                var aotBytesPath = PathUtil.GetCombinePath(aotSaveDir, $"{aot}.bytes");
                File.Copy(aotPath, aotBytesPath, true);
            }

            return failureList.ToArray();
        }

        private static UnityEditor.Build.NamedBuildTarget GetCurrentNamedBuildTarget()
        {
#if UNITY_ANDROID
            return UnityEditor.Build.NamedBuildTarget.Android;
#elif UNITY_IOS
            return UnityEditor.Build.NamedBuildTarget.iOS;
#elif UNITY_STANDALONE
            return UnityEditor.Build.NamedBuildTarget.Standalone;
#elif UNITY_WEBGL
            return UnityEditor.Build.NamedBuildTarget.WebGL;
#else
            return UnityEditor.Build.NamedBuildTarget.Unknown;
#endif
        }
    }
}