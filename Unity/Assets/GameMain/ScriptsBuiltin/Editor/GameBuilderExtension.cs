using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace GameMain.Editor
{
    public class GameBuilderExtension
    {
        [MenuItem("Game Framework/Resource Tools/Resolve Duplicate Assets【解决AB资源重复依赖冗余】", false, 100)]
        static async void RefreshSharedAssets()
        {
            await AutoResolveAbDuplicateAssets();
        }

        public static async UniTask<bool> AutoResolveAbDuplicateAssets()
        {
            ResourceEditorController resEditor = new ResourceEditorController();
            bool resolved = false;
            bool refreshCompleted = false;
            resEditor.OnLoadCompleted += async () =>
            {
                if (resEditor.HasResource(Constant.SharedAssetBundleName, null))
                {
                    foreach (var item in resEditor.GetResource(Constant.SharedAssetBundleName, null).GetAssets())
                    {
                        resEditor.UnassignAsset(item.Guid);
                    }

                    resEditor.Save();
                }

                var duplicateAssetNames = await FindDuplicateAssetNamesAsync();
                resolved = ResolveDuplicateAssets(resEditor, duplicateAssetNames);
                refreshCompleted = true;
            };
            if (!resEditor.Load())
            {
                refreshCompleted = true;
            }

            await UniTask.WaitUntil(() => refreshCompleted);
            return resolved;
        }

        private static bool ResolveDuplicateAssets(ResourceEditorController resEditor, List<string> duplicateAssetNames)
        {
            if (!resEditor.HasResource(Constant.SharedAssetBundleName, null))
            {
                bool addSuccess = resEditor.AddResource(Constant.SharedAssetBundleName, null, null, LoadType.LoadFromMemoryAndQuickDecrypt, false);

                if (!addSuccess)
                {
                    Debug.LogWarningFormat("ResourceEditor Add Resource:{0} Failed!", Constant.SharedAssetBundleName);
                    return false;
                }
            }

            var sharedRes = resEditor.GetResource(Constant.SharedAssetBundleName, null);
            bool hasChanged = false;
            List<string> sharedResFiles = new List<string>();
            foreach (var item in sharedRes.GetAssets())
            {
                sharedResFiles.Add(item.Name);
            }

            Debug.Log($"-------------添加下列冗余资源到{Constant.SharedAssetBundleName}------------");
            foreach (var assetName in duplicateAssetNames)
            {
                Debug.Log($"冗余资源:{assetName}");
                if (sharedResFiles.Contains(assetName))
                {
                    continue;
                }

                if (!resEditor.AssignAsset(AssetDatabase.AssetPathToGUID(assetName), Constant.SharedAssetBundleName, null))
                {
                    Debug.LogWarning($"添加资源:{assetName}到{Constant.SharedAssetBundleName}失败!");
                }

                hasChanged = true;
            }

            Debug.Log($"-------------处理冗余资源结束------------");
            var sharedAseets = sharedRes.GetAssets();
            for (int i = sharedAseets.Length - 1; i >= 0; i--)
            {
                var asset = sharedAseets[i];
                if (!duplicateAssetNames.Contains(asset.Name))
                {
                    if (!resEditor.UnassignAsset(asset.Guid))
                    {
                        Debug.LogWarning($"移除{Constant.SharedAssetBundleName}中的资源:{asset.Name}失败!");
                    }

                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                resEditor.RemoveUnknownAssets();
                resEditor.RemoveUnusedResources();
                return resEditor.Save();
            }

            return true;
        }

        private static async UniTask<List<string>> FindDuplicateAssetNamesAsync()
        {
            List<string> duplicateAssets = new List<string>();
            bool refreshCompleted = false;
            ResourceAnalyzerController resAnalyzer = new ResourceAnalyzerController();
            resAnalyzer.OnAnalyzeCompleted += () =>
            {
                var scatteredAssetNames = resAnalyzer.GetScatteredAssetNames();
                foreach (var scatteredAsset in scatteredAssetNames)
                {
                    var hostAssets = resAnalyzer.GetHostAssets(scatteredAsset);
                    if (hostAssets == null || hostAssets.Length < 1) continue;
                    var defaultHostAsset = hostAssets.FirstOrDefault(res => res.Resource.FullName != Constant.SharedAssetBundleName);
                    if (defaultHostAsset != null)
                    {
                        var hostResourceName = defaultHostAsset.Resource.FullName;
                        foreach (var hostAsset in hostAssets)
                        {
                            if (hostAsset.Resource.FullName == Constant.SharedAssetBundleName) continue;
                            if (hostResourceName != hostAsset.Resource.Name)
                            {
                                duplicateAssets.Add(scatteredAsset);
                                break;
                            }
                        }
                    }
                }

                refreshCompleted = true;
            };
            if (resAnalyzer.Prepare())
            {
                resAnalyzer.Analyze();
            }
            else
            {
                refreshCompleted = true;
            }

            await UniTask.WaitUntil(() => refreshCompleted);
            return duplicateAssets;
        }

        /// <summary>
        /// 移除StreamingAssets目录的AB包
        /// </summary>
        internal static void RemoveStreamingAssetsBundles()
        {
            var streamingAssetsPath = Application.streamingAssetsPath;
            if (Directory.Exists(streamingAssetsPath))
            {
                var oldAbFiles = Directory.GetFiles(streamingAssetsPath, "*.dat", SearchOption.AllDirectories);
                var projectRoot = Directory.GetParent(Application.dataPath).FullName;
                foreach (var abFile in oldAbFiles)
                {
                    Debug.Log($"删除文件:{abFile}");
                    var relativePath = Path.GetRelativePath(projectRoot, abFile);
                    AssetDatabase.DeleteAsset(relativePath);
                }

                var dirInfo = new DirectoryInfo(streamingAssetsPath);
                var subDirs = dirInfo.GetDirectories("*", SearchOption.AllDirectories);
                foreach (var item in subDirs)
                {
                    if (!item.Exists) continue;
                    if (item.GetFiles("*", SearchOption.AllDirectories).Length <= 0)
                    {
                        Debug.Log($"删除文件夹:{item}");

                        var relativePath = Path.GetRelativePath(projectRoot, item.FullName);
                        AssetDatabase.DeleteAsset(relativePath);
                    }
                }
            }
        }
    }
}