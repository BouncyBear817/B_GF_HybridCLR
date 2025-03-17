using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using GameMain.Builtin;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using static UnityEditor.BuildPlayerWindow;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace GameMain.Editor
{
    public class GameBuilder : EditorWindow
    {
        private const string BUILD_TASK_TAG = "BUILD_TASK_TAG";

        private string[] KeystoreExtensionNames =
        {
            ".keystore", ".jks", ".ks"
        };

        private ResourceBuilderController mController;
        private bool mOrderBuildResources;
        private int mCompressionHelperTypeNameIndex;
        private int mBuildEventHandlerTypeNameIndex;

        private Vector2 mScrollPosition;

        public static void Open()
        {
            var window = GetWindow<GameBuilder>("Game Builder", true);
            window.minSize = new Vector2(800f, 800f);
        }

        private void OnEnable()
        {
            RefreshHybridCLRDefine();

            mController = new ResourceBuilderController();
            mController.OnLoadingResource += OnLoadingResource;
            mController.OnLoadingAsset += OnLoadingAsset;
            mController.OnLoadCompleted += OnLoadCompleted;
            mController.OnAnalyzingAsset += OnAnalyzingAsset;
            mController.OnAnalyzeCompleted += OnAnalyzeCompleted;
            mController.ProcessingAssetBundle += OnProcessingAssetBundle;
            mController.ProcessingBinary += OnProcessingBinary;
            mController.ProcessResourceComplete += OnProcessResourceComplete;
            mController.BuildResourceError += OnBuildResourceError;

            mOrderBuildResources = false;

            if (mController.Load())
            {
                Debug.Log("Load configuration success.");

                mCompressionHelperTypeNameIndex = 0;
                string[] compressionHelperTypeNames = mController.GetCompressionHelperTypeNames();
                for (int i = 0; i < compressionHelperTypeNames.Length; i++)
                {
                    if (mController.CompressionHelperTypeName == compressionHelperTypeNames[i])
                    {
                        mCompressionHelperTypeNameIndex = i;
                        break;
                    }
                }

                mController.RefreshCompressionHelper();

                mBuildEventHandlerTypeNameIndex = 0;
                string[] buildEventHandlerTypeNames = mController.GetBuildEventHandlerTypeNames();
                for (int i = 0; i < buildEventHandlerTypeNames.Length; i++)
                {
                    if (mController.BuildEventHandlerTypeName == buildEventHandlerTypeNames[i])
                    {
                        mBuildEventHandlerTypeNameIndex = i;
                        break;
                    }
                }

                mController.RefreshBuildEventHandler();
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }

            if (string.IsNullOrWhiteSpace(mController.OutputDirectory) || !Directory.Exists(mController.OutputDirectory))
            {
                mController.OutputDirectory = Constant.AssetBundleOutputPath;
            }

            if (GameBuildSettings.GetInstance().ResourceMode != ResourceMode.Unspecified)
            {
                SetResourceMode(GameBuildSettings.GetInstance().ResourceMode);
            }
        }

        private void Update()
        {
            if (mOrderBuildResources)
            {
                mOrderBuildResources = false;
                BuildResources();
            }

            if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && EditorPrefs.GetBool(BUILD_TASK_TAG, false))
            {
                EditorPrefs.SetBool(BUILD_TASK_TAG, false);
                CallBuildMethods();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Environment Information", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Product Name", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.ProductName);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Company Name", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.CompanyName);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Game Identifier", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.GameIdentifier);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Game Framework Version", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.GameFrameworkVersion);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Unity Version", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.UnityVersion);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Applicable Game Version", GUILayout.Width(160f));
                            EditorGUILayout.LabelField(mController.ApplicableGameVersion);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.LabelField("Platforms", EditorStyles.boldLabel);
                            EditorGUILayout.BeginHorizontal("box");
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    DrawPlatform(Platform.Windows, "Windows");
                                    DrawPlatform(Platform.Windows64, "Windows x64");
                                    DrawPlatform(Platform.MacOS, "macOS");
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.BeginVertical();
                                {
                                    DrawPlatform(Platform.Linux, "Linux");
                                    DrawPlatform(Platform.IOS, "iOS");
                                    DrawPlatform(Platform.Android, "Android");
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.BeginVertical();
                                {
                                    DrawPlatform(Platform.WindowsStore, "Windows Store");
                                    DrawPlatform(Platform.WebGL, "WebGL");
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Compression", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("AssetBundle Compression", GUILayout.Width(160f));
                            mController.AssetBundleCompression = (AssetBundleCompressionType)EditorGUILayout.EnumPopup(mController.AssetBundleCompression);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Compression Helper", GUILayout.Width(160f));
                            string[] names = mController.GetCompressionHelperTypeNames();
                            int selectedIndex = EditorGUILayout.Popup(mCompressionHelperTypeNameIndex, names);
                            if (selectedIndex != mCompressionHelperTypeNameIndex)
                            {
                                mCompressionHelperTypeNameIndex = selectedIndex;
                                mController.CompressionHelperTypeName = selectedIndex <= 0 ? string.Empty : names[selectedIndex];
                                if (mController.RefreshCompressionHelper())
                                {
                                    Debug.Log("Set compression helper success.");
                                }
                                else
                                {
                                    Debug.LogWarning("Set compression helper failure.");
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Additional Compression", GUILayout.Width(160f));
                            mController.AdditionalCompressionSelected =
                                EditorGUILayout.ToggleLeft("Additional Compression for Output Full Resources with Compression Helper", mController.AdditionalCompressionSelected);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
                        if (GUILayout.Button("Resource Editor", GUILayout.Width(120f)))
                        {
                            var editorClass = Utility.Assembly.GetType("UnityGameFramework.Editor.ResourceTools.ResourceEditor");
                            editorClass?.GetMethod("Open", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
                            GUIUtility.ExitGUI();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Force Rebuild AssetBundle", GUILayout.Width(160f));
                            mController.ForceRebuildAssetBundleSelected = EditorGUILayout.Toggle(mController.ForceRebuildAssetBundleSelected);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Build Event Handler", GUILayout.Width(160f));
                            string[] names = mController.GetBuildEventHandlerTypeNames();
                            int selectedIndex = EditorGUILayout.Popup(mBuildEventHandlerTypeNameIndex, names);
                            if (selectedIndex != mBuildEventHandlerTypeNameIndex)
                            {
                                mBuildEventHandlerTypeNameIndex = selectedIndex;
                                mController.BuildEventHandlerTypeName = selectedIndex <= 0 ? string.Empty : names[selectedIndex];
                                if (mController.RefreshBuildEventHandler())
                                {
                                    Debug.Log("Set build event handler success.");
                                }
                                else
                                {
                                    Debug.LogWarning("Set build event handler failure.");
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Internal Resource Version", GUILayout.Width(160f));
                            mController.InternalResourceVersion = EditorGUILayout.IntField(mController.InternalResourceVersion);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Resource Version", GUILayout.Width(160f));
                            GUILayout.Label(Utility.Text.Format("{0} ({1})", mController.ApplicableGameVersion, mController.InternalResourceVersion));
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Resource Mode", GUILayout.Width(160f));
                            EditorGUI.BeginChangeCheck();
                            {
                                GameBuildSettings.GetInstance().ResourceMode = (ResourceMode)EditorGUILayout.EnumPopup(GameBuildSettings.GetInstance().ResourceMode, GUILayout.Width(160f));
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                RefreshHybridCLRDefine();
                                SetResourceMode(GameBuildSettings.GetInstance().ResourceMode);
                            }

                            if (GameBuildSettings.GetInstance().ResourceMode == ResourceMode.Updatable || GameBuildSettings.GetInstance().ResourceMode == ResourceMode.UpdatableWhilePlaying)
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("Resource Version File Name", GUILayout.Width(160f));
                                GameBuildSettings.GetInstance().ResourceVersionFileName = EditorGUILayout.TextField(GameBuildSettings.GetInstance().ResourceVersionFileName);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (GameBuildSettings.GetInstance().ResourceMode == ResourceMode.Unspecified)
                        {
                            EditorGUILayout.HelpBox("ResourceMode is invalid.", MessageType.Error);
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Output Directory", GUILayout.Width(160f));
                            mController.OutputDirectory = EditorGUILayout.TextField(mController.OutputDirectory);
                            if (GUILayout.Button("Browse...", GUILayout.Width(80f)))
                            {
                                BrowseOutputDirectory();
                            }

                            if (GUILayout.Button("Reveal In Finder", GUILayout.Width(120f)))
                            {
                                EditorUtility.RevealInFinder(mController.OutputDirectory);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Working Path", GUILayout.Width(160f));
                            GUILayout.Label(mController.WorkingPath);
                        }
                        EditorGUILayout.EndHorizontal();

                        SetOutputResourcePathByResourceMode(GameBuildSettings.GetInstance().ResourceMode);

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Build Report Path", GUILayout.Width(160f));
                            GUILayout.Label(mController.BuildReportPath);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    GetBuildMessage(out var buildMessage, out var buildMessageType);
                    EditorGUILayout.HelpBox(buildMessage, buildMessageType);

                    DrawHotfixSettingsPanel();

                    DrawBuildSettingsPanel();
                }
                EditorGUILayout.EndScrollView();

                DrawButton();
            }
            EditorGUILayout.EndVertical();
        }

        private void RefreshHybridCLRDefine()
        {
            switch (GameBuildSettings.GetInstance().ResourceMode)
            {
                case ResourceMode.Package:
#if !DISABLE_HYBRIDCLR
                    HybridCLRExtension.RefreshHybridCLRDisable();
#endif
                    break;
                case ResourceMode.Updatable:
                case ResourceMode.UpdatableWhilePlaying:
#if DISABLE_HYBRIDCLR
                    HybridCLRExtension.RefreshHybridCLREnable();
#endif
                    break;
            }
        }

        private void SetResourceMode(ResourceMode resourceMode)
        {
            if (resourceMode == ResourceMode.Unspecified)
            {
                return;
            }

            mController.OutputPackageSelected = false;
            mController.OutputFullSelected = false;
            mController.OutputPackedSelected = false;

            switch (resourceMode)
            {
                case ResourceMode.Package:
                    mController.OutputPackageSelected = true;
                    break;

                case ResourceMode.Updatable:
                case ResourceMode.UpdatableWhilePlaying:
                    mController.OutputFullSelected = true;
                    break;
            }
        }

        private void SetOutputResourcePathByResourceMode(ResourceMode resourceMode)
        {
            switch (resourceMode)
            {
                case ResourceMode.Package:
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Output Package Path", GUILayout.Width(160f));
                        GUILayout.Label(mController.OutputPackagePath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                    break;
                case ResourceMode.Updatable:
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Output Full Path", GUILayout.Width(160f));
                        GUILayout.Label(mController.OutputFullPath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                    break;
                case ResourceMode.UpdatableWhilePlaying:
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Output Packed Path", GUILayout.Width(160f));
                        GUILayout.Label(mController.OutputPackedPath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                    break;
            }
        }

        private void BrowseOutputDirectory()
        {
            string directory = EditorUtility.OpenFolderPanel("Select Output Directory", mController.OutputDirectory, string.Empty);
            if (!string.IsNullOrEmpty(directory))
            {
                mController.OutputDirectory = directory;
            }
        }

        private void GetBuildMessage(out string message, out MessageType messageType)
        {
            message = string.Empty;
            messageType = MessageType.Error;
            if (mController.Platforms == Platform.Undefined)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Platform is invalid.";
            }

            if (string.IsNullOrEmpty(mController.CompressionHelperTypeName))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Compression helper is invalid.";
            }

            if (!mController.IsValidOutputDirectory)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Output directory is invalid.";
            }

            if (!string.IsNullOrEmpty(message))
            {
                return;
            }

            messageType = MessageType.Info;
            if (Directory.Exists(mController.OutputPackagePath))
            {
                message += Utility.Text.Format("{0} will be overwritten.", mController.OutputPackagePath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(mController.OutputFullPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", mController.OutputFullPath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(mController.OutputPackedPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", mController.OutputPackedPath);
                messageType = MessageType.Warning;
            }

            if (messageType == MessageType.Warning)
            {
                return;
            }

            message = "Ready to build.";
        }

        private void DrawHotfixSettingsPanel()
        {
            if (mController.OutputFullSelected || mController.OutputPackedSelected)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Hotfix Settings : ", EditorStyles.boldLabel, GUILayout.Width(160f));

                    if (GUILayout.Button("Hotfix Settings", GUILayout.Width(120f)))
                    {
                        SettingsService.OpenProjectSettings("Project/HybridCLR Settings");
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Applicable Game Version", GUILayout.Width(160f));
                        GameBuildSettings.GetInstance().ApplicableGameVersion = EditorGUILayout.TextField(GameBuildSettings.GetInstance().ApplicableGameVersion);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("App Update Uri", GUILayout.Width(160f));
                        GameBuildSettings.GetInstance().AppUpdateUri = EditorGUILayout.TextField(GameBuildSettings.GetInstance().AppUpdateUri);
                        GameBuildSettings.GetInstance().ForceUpdateApp = EditorGUILayout.ToggleLeft("Force Update", GameBuildSettings.GetInstance().ForceUpdateApp, GUILayout.Width(100f));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("App Update Desc", GUILayout.Width(160f));
                    GameBuildSettings.GetInstance().AppUpdateDesc = EditorGUILayout.TextArea(GameBuildSettings.GetInstance().AppUpdateDesc, GUILayout.Height(50f));
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBuildSettingsPanel()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Build App Settings : ", EditorStyles.boldLabel, GUILayout.Width(160f));
#if UNITY_ANDROID
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Build App Bundle(GP)", GUILayout.Width(160f));
                    EditorUserBuildSettings.buildAppBundle = EditorGUILayout.ToggleLeft("", EditorUserBuildSettings.buildAppBundle);
                }
                EditorGUILayout.EndHorizontal();
#endif
                GameBuildSettings.GetInstance().DebugMode = EditorGUILayout.ToggleLeft("Debug Mode", GameBuildSettings.GetInstance().DebugMode);

                if (GUILayout.Button("Player Settings", GUILayout.Width(120f)))
                {
                    SettingsService.OpenProjectSettings("Project/Player");
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("App Version", GUILayout.Width(160f));
                    PlayerSettings.bundleVersion = EditorGUILayout.TextField(PlayerSettings.bundleVersion);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("App Build Path", GUILayout.Width(160f));
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        GameBuildSettings.GetInstance().AppBuildPath = EditorGUILayout.TextField(GameBuildSettings.GetInstance().AppBuildPath);
                        if (GUILayout.Button("Select", GUILayout.Width(120f)))
                        {
                            var folder = Path.Combine(Application.dataPath, EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget));
                            Debug.Log(EditorUserBuildSettings.activeBuildTarget);
                            if (!Directory.Exists(folder))
                            {
                                folder = Application.dataPath;
                            }

                            var path = EditorUtility.OpenFolderPanel("Select Build Game Path", folder, "");
                            if (!string.IsNullOrEmpty(path))
                            {
                                GameBuildSettings.GetInstance().AppBuildPath = path;
                            }

                            GUIUtility.ExitGUI();
                        }

                        if (changeCheckScope.changed)
                        {
                            EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, GameBuildSettings.GetInstance().AppBuildPath);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

#if UNITY_ANDROID
                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Bundle Version Code", GUILayout.Width(160f));
                    PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField(PlayerSettings.Android.bundleVersionCode);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Use Custom Keystore", GUILayout.Width(160f));
                    PlayerSettings.Android.useCustomKeystore = EditorGUILayout.ToggleLeft("", PlayerSettings.Android.useCustomKeystore);
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(!PlayerSettings.Android.useCustomKeystore);
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(160f));
                        PlayerSettings.Android.keystoreName = EditorGUILayout.TextField(PlayerSettings.Android.keystoreName);
                        if (GUILayout.Button("Select Keystore", GUILayout.Width(120f)))
                        {
                            var folder = Path.Combine(Application.dataPath, PlayerSettings.Android.keystoreName);
                            if (!Directory.Exists(folder))
                            {
                                folder = Application.dataPath;
                            }

                            var path = EditorUtility.OpenFilePanel("Select Keystore", folder, "keystore,jks,ks");
                            if (!string.IsNullOrEmpty(path))
                            {
                                PlayerSettings.Android.keystoreName = path.Replace(Application.dataPath + "/", "");
                            }

                            GUIUtility.ExitGUI();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (PlayerSettings.Android.useCustomKeystore)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Keystore Password", GUILayout.Width(160f));
                        PlayerSettings.keystorePass = EditorGUILayout.PasswordField(PlayerSettings.keystorePass);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Key Alias Name", GUILayout.Width(160f));
                        PlayerSettings.Android.keyaliasName = EditorGUILayout.TextField(PlayerSettings.Android.keyaliasName);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Key Alias Password", GUILayout.Width(160f));
                        PlayerSettings.keyaliasPass = EditorGUILayout.PasswordField(PlayerSettings.keyaliasPass);
                    }
                    EditorGUILayout.EndHorizontal();
                }
#elif UNITY_IOS
                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Build Number", GUILayout.Width(160f));
                    PlayerSettings.iOS.buildNumber = EditorGUILayout.TextField(PlayerSettings.iOS.buildNumber);
                }
                EditorGUILayout.EndHorizontal();
#endif
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawButton()
        {
            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUI.BeginDisabledGroup(mController.Platforms == Platform.Undefined || string.IsNullOrEmpty(mController.CompressionHelperTypeName) || !mController.IsValidOutputDirectory);
                {
                    if (GUILayout.Button("Build Resources", GUILayout.Height(35f)))
                    {
                        if (EditorUtility.DisplayDialog("Build Resources", $"Resources Version : {mController.InternalResourceVersion}", "Build", "Cancel"))
                        {
                            BuildHotfix();
                        }
                    }

                    if (GameBuildSettings.GetInstance().ResourceMode == ResourceMode.Package)
                    {
                        if (GUILayout.Button("Build App", GUILayout.Height(35f)))
                        {
                            if (EditorUtility.DisplayDialog("Build App", $"App Version : {Application.version}", "Build", "Cancel"))
                            {
                                BuildApp(false);
                                GUIUtility.ExitGUI();
                            }
                        }
                    }
                    else if (GameBuildSettings.GetInstance().ResourceMode == ResourceMode.Updatable || GameBuildSettings.GetInstance().ResourceMode == ResourceMode.UpdatableWhilePlaying)
                    {
                        GUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("Build App", GUILayout.Height(15f)))
                            {
                                if (EditorUtility.DisplayDialog("Build App", $"App Version : {Application.version}", "Build", "Cancel"))
                                {
                                    BuildApp(false);
                                    GUIUtility.ExitGUI();
                                }
                            }

                            if (GUILayout.Button("Full Build App (Generate Aot Dlls)", GUILayout.Height(15f)))
                            {
                                if (EditorUtility.DisplayDialog("Full Build App (Generate Aot Dlls)", $"App Version : {Application.version}", "Build", "Cancel"))
                                {
                                    BuildApp(true);
                                }
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Save", GUILayout.Height(35f)))
                {
                    SaveConfiguration();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private async void BuildApp(bool generateAot)
        {
#if UNITY_ANDROID
            if (PlayerSettings.Android.useCustomKeystore && !CheckKeystoreAvailable(PlayerSettings.Android.keystoreName))
            {
                EditorUtility.DisplayDialog("Build App Error!", $"Keystore file does not exist or has an incorrect format : {PlayerSettings.Android.keystoreName}", "OK");
                return;
            }
#endif
            mController.OutputPackedSelected = GameBuildSettings.GetInstance().ResourceMode != ResourceMode.Package && await HasPackedResource();
            if (mController.OutputPackageSelected)
            {
                await GameBuilderExtension.AutoResolveAbDuplicateAssets();
                if (mController.BuildResources())
                {
                    DeleteAotDlls(); // 单机模式删除Resource下的AOT dlls
                    PrepareBuildApp(generateAot);
                }
            }
            else
            {
                GameBuilderExtension.RemoveStreamingAssetsBundles();
                var buildAppReady = !mController.OutputPackedSelected;
                if (mController.OutputPackedSelected)
                {
#if !DISABLE_HYBRIDCLR
                    HybridCLRExtension.CompileTargetDll(false);
#endif
                    await GameBuilderExtension.AutoResolveAbDuplicateAssets();
                    buildAppReady = mController.BuildResources();
                }

                if (buildAppReady)
                {
                    PrepareBuildApp(generateAot);
                }
            }
        }

        private void PrepareBuildApp(bool generateAot)
        {
#if !DISABLE_HYBRIDCLR
            GenerateHotfixCodeStripConfig(false);
            HybridCLRGenerateAll(generateAot);
#else
            GenerateHotfixCodeStripConfig(true);
#endif
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(BUILD_TASK_TAG, true);
        }

        /// <summary>
        /// 生成或删除热更dlls防裁剪的link.xml
        /// 单机模式时需把热更dlls打到包里
        /// </summary>
        /// <param name="b">true生成; false删除</param>
        private void GenerateHotfixCodeStripConfig(bool b)
        {
            var linkDir = Path.GetDirectoryName(Constant.HotfixAssembly);
            var linkFile = PathUtil.GetCombinePath(linkDir, "link.xml");
            if (b)
            {
                var strBuilder = new StringBuilder();
                strBuilder.AppendLine("<linker>");
                foreach (var dllName in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesIncludePreserved)
                {
                    strBuilder.AppendLine(Utility.Text.Format("\t<assembly fullname=\"{0}\" preserve=\"all\" />", dllName));
                }

                strBuilder.AppendLine("</linker>");
                File.WriteAllText(linkFile, strBuilder.ToString());
            }
            else
            {
                if (File.Exists(linkFile))
                {
                    File.Delete(linkFile); // 热更包不需要添加防裁剪
                }
            }
        }

        private void HybridCLRGenerateAll(bool generateAot)
        {
            var installer = new HybridCLR.Editor.Installer.InstallerController();
            if (!installer.HasInstalledHybridCLR())
            {
                throw new BuildFailedException("You have not initialized HybridCLR, please install it via menu 'HybridCLR/Installer'.");
            }

            HybridCLRExtension.CompileTargetDll(false);
            Il2CppDefGeneratorCommand.GenerateIl2CppDef();

            var target = EditorUserBuildSettings.activeBuildTarget;
            LinkGeneratorCommand.GenerateLinkXml(target);
            if (generateAot)
            {
                // 生成裁剪后的aot dlls
                StripAOTDllCommand.GenerateStripedAOTDlls(target);
                HybridCLRExtension.CopyAotDllsToProject(target);
            }

            // 桥接函数生后依赖于aot dll, 必须保证已经build过，生产aot dll
            MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(target);
            AOTReferenceGeneratorCommand.GenerateAOTGenericReference(target);
        }

        private void DeleteAotDlls()
        {
            var aotSaveDir = "Resources/AotDlls";
            if (Directory.Exists(aotSaveDir))
            {
                AssetDatabase.DeleteAsset(aotSaveDir);
            }
        }

        private bool CheckKeystoreAvailable(string keystore)
        {
            if (string.IsNullOrEmpty(keystore))
            {
                return false;
            }

            var extension = Path.GetExtension(keystore).ToLower();
            if (File.Exists(keystore) && KeystoreExtensionNames.Contains(extension))
            {
                return true;
            }

            return false;
        }

        private async UniTask<bool> HasPackedResource()
        {
            var resEditor = new ResourceEditorController();
            var loaded = false;
            var result = false;
            resEditor.OnLoadCompleted += () =>
            {
                var bundles = resEditor.GetResources();
                foreach (var bundle in bundles)
                {
                    if (bundle.Packed)
                    {
                        result = true;
                    }
                }

                loaded = true;
            };

            if (resEditor.Load())
            {
                await UniTask.WaitUntil(() => true == loaded);
            }
            else
            {
                loaded = true;
            }

            return result;
        }

        private async void BuildHotfix()
        {
            mController.OutputPackedSelected = GameBuildSettings.GetInstance().ResourceMode != ResourceMode.Package && await HasPackedResource();
            await GameBuilderExtension.AutoResolveAbDuplicateAssets();

#if !DISABLE_HYBRIDCLR
            HybridCLRExtension.CompileTargetDll();
#endif
            mOrderBuildResources = true;
        }

        private void BuildResources()
        {
            if (mController.BuildResources())
            {
                Debug.Log("Build resources success.");
                SaveConfiguration();
            }
            else
            {
                Debug.LogWarning("Build resources failure.");
            }
        }

        private void SaveConfiguration()
        {
            EditorUtility.SetDirty(GameBuildSettings.GetInstance());
            if (mController.Save())
            {
                Debug.Log("Save configuration success.");
            }
            else
            {
                Debug.LogWarning("Save configuration failure.");
            }
        }

        private void CallBuildMethod()
        {
            typeof(BuildPlayerWindow).GetField("buildCompletionHandler", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Action<BuildReport>(OnPostProcessBuild));
            var getBuildPlayerOptions = typeof(DefaultBuildMethods).GetMethod("GetBuildPlayerOptionsInternal", BindingFlags.NonPublic | BindingFlags.Static);

            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            buildPlayerOptions.target = (BuildTarget)Utility.Assembly.GetType("UnityEditor.EditorUserBuildSettingsUtils")
                .GetMethod("CalculateSelectedBuildTarget", BindingFlags.Public | BindingFlags.Static)
                ?.Invoke(null, null);
            buildPlayerOptions.subtarget = (int)typeof(EditorUserBuildSettings).GetMethod("GetSelectedSubtargetFor", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(null, new object[] { buildPlayerOptions.target });

            var errorBuildDir = string.IsNullOrWhiteSpace(GameBuildSettings.GetInstance().AppBuildPath);
            var locationPathName = GetBuildLocation(buildPlayerOptions.targetGroup, buildPlayerOptions.target, buildPlayerOptions.subtarget, buildPlayerOptions.options);
            var locationDir = Path.GetDirectoryName(locationPathName);
            if (locationDir != null && !Directory.Exists(locationDir))
            {
                Directory.CreateDirectory(locationDir);
            }

            EditorUserBuildSettings.SetBuildLocation(buildPlayerOptions.target, locationPathName);

            buildPlayerOptions = (BuildPlayerOptions)getBuildPlayerOptions.Invoke(null, new object[] { errorBuildDir, buildPlayerOptions });
            buildPlayerOptions.locationPathName = locationPathName;
            var scenesList = new ArrayList();
            var editorScenes = EditorBuildSettings.scenes;
            foreach (var scene in editorScenes)
            {
                if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                {
                    scenesList.Add(scene.path);
                    break;
                }
            }
            EditorUserBuildSettings.buildScriptsOnly = false;
            buildPlayerOptions.scenes = scenesList.ToArray(typeof(string)) as string[];
            DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
        
        private void CallBuildMethods()
        {
            typeof(UnityEditor.BuildPlayerWindow).GetField("buildCompletionHandler", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Action<BuildReport>(OnPostProcessBuild));
            var getBuildPlayerOptions = typeof(UnityEditor.BuildPlayerWindow.DefaultBuildMethods).GetMethod("GetBuildPlayerOptionsInternal", BindingFlags.NonPublic | BindingFlags.Static);
            var buildOptions = new BuildPlayerOptions();
            buildOptions.options = BuildOptions.ShowBuiltPlayer;

            buildOptions.targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            buildOptions.target = (BuildTarget)Utility.Assembly.GetType("UnityEditor.EditorUserBuildSettingsUtils").GetMethod("CalculateSelectedBuildTarget", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            buildOptions.subtarget = (int)typeof(UnityEditor.EditorUserBuildSettings).GetMethod("GetSelectedSubtargetFor", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { buildOptions.target });
            var errBuildDir = string.IsNullOrWhiteSpace(GameBuildSettings.GetInstance().AppBuildPath);
            var locationPathName = GetBuildLocation(buildOptions.targetGroup, buildOptions.target, buildOptions.subtarget, buildOptions.options);
            var locationDir = Path.GetDirectoryName(locationPathName);
            if (!Directory.Exists(locationDir))
            {
                Directory.CreateDirectory(locationDir);
            }
            EditorUserBuildSettings.SetBuildLocation(buildOptions.target, locationPathName);

            buildOptions = (BuildPlayerOptions)getBuildPlayerOptions.Invoke(null, new object[] { errBuildDir, buildOptions });
            buildOptions.locationPathName = locationPathName;
            ArrayList scenesList = new ArrayList();
            EditorBuildSettingsScene[] editorScenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in editorScenes)
            {
                if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                {
                    scenesList.Add(scene.path);
                }
            }
            // EditorUserBuildSettings.buildScriptsOnly = false;
            buildOptions.scenes = scenesList.ToArray(typeof(string)) as string[];
            DefaultBuildMethods.BuildPlayer(buildOptions);
        }

        private string GetBuildLocation(BuildTargetGroup targetGroup, BuildTarget target, int subtarget, BuildOptions options)
        {
            var defaultFolder = PathUtil.GetCombinePath(GameBuildSettings.GetInstance().AppBuildPath, target.ToString());
            var defaultName = Application.productName;
            var extension = Utility.Assembly.GetType("UnityEditor.PostprocessBuildPlayer")
                .GetMethod("GetExtensionForBuildTarget", new Type[] { typeof(BuildTargetGroup), typeof(BuildTarget), typeof(int), typeof(BuildOptions) })
                .Invoke(null, new object[] { targetGroup, target, subtarget, options }) as string;
            var buildPath = defaultFolder;
            if (!string.IsNullOrEmpty(extension))
            {
                var appFileName = $"{defaultName}.{extension}";
                buildPath = PathUtil.GetCombinePath(defaultFolder, appFileName);
            }

            return buildPath;
        }

        private void OnPostProcessBuild(BuildReport report)
        {
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"Build App Failed ： {report.summary.result}");
                return;
            }

            // rename app
            if (report.summary.platform == BuildTarget.Android | report.summary.platform == BuildTarget.iOS)
            {
                var appFile = report.summary.outputPath;
                if (File.Exists(appFile))
                {
                    var dir = Path.GetDirectoryName(appFile);
                    var name = Path.GetFileNameWithoutExtension(appFile);
                    var extension = Path.GetExtension(appFile);
                    var debugMode = GameBuildSettings.GetInstance().DebugMode ? "debug" : "release";
                    var development = EditorUserBuildSettings.development ? "Dev" : string.Empty;
                    var finalName = $"{name}_{debugMode}{development}_v{Application.version}{extension}";
                    finalName = Path.Combine(dir, finalName);

                    if (File.Exists(finalName))
                    {
                        File.Decrypt(finalName);
                    }

                    File.Move(report.summary.outputPath, finalName);
                }
            }
        }

        private void DrawPlatform(Platform platform, string platformName)
        {
            mController.SelectPlatform(platform, EditorGUILayout.ToggleLeft(platformName, mController.IsPlatformSelected(platform)));
        }

        private void OnLoadingResource(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Resources", Utility.Text.Format("Loading resources, {0}/{1} loaded.", index, count), (float)index / count);
        }

        private void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets", Utility.Text.Format("Loading assets, {0}/{1} loaded.", index, count), (float)index / count);
        }

        private void OnLoadCompleted()
        {
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            mController.OutputDirectory = Path.GetFullPath(GameBuildSettings.GetInstance().ABBuildPath, projectRoot);
            EditorUtility.ClearProgressBar();
        }

        private void OnAnalyzingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Analyzing Assets", Utility.Text.Format("Analyzing assets, {0}/{1} analyzed.", index, count), (float)index / count);
        }

        private void OnAnalyzeCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private bool OnProcessingAssetBundle(string assetBundleName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing AssetBundle", Utility.Text.Format("Processing '{0}'...", assetBundleName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                Repaint();
                return false;
            }
        }

        private bool OnProcessingBinary(string binaryName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing Binary", Utility.Text.Format("Processing '{0}'...", binaryName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                Repaint();
                return false;
            }
        }

        private void OnProcessResourceComplete(Platform platform)
        {
            EditorUtility.ClearProgressBar();
            Debug.Log(Utility.Text.Format("Build resources for '{0}' complete.", platform));
        }

        private void OnBuildResourceError(string errorMessage)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogWarning(Utility.Text.Format("Build resources error with error message '{0}'.", errorMessage));
        }
    }
}