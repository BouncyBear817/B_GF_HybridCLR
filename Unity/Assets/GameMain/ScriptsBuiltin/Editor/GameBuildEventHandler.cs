using System.IO;
using GameMain.Builtin;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace GameMain.Editor
{
    public class GameBuildEventHandler : IBuildEventHandler
    {
        public bool ContinueOnFailure => false;

        private readonly VersionInfo mVersionInfo = new VersionInfo();

        /// <summary>
        /// 所有平台生成开始前的预处理事件。
        /// </summary>
        /// <param name="productName">产品名称。</param>
        /// <param name="companyName">公司名称。</param>
        /// <param name="gameIdentifier">游戏识别号。</param>
        /// <param name="gameFrameworkVersion">游戏框架版本。</param>
        /// <param name="unityVersion">Unity 版本。</param>
        /// <param name="applicableGameVersion">适用游戏版本。</param>
        /// <param name="internalResourceVersion">内部资源版本。</param>
        /// <param name="platforms">生成的目标平台。</param>
        /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
        /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
        /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
        /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
        /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
        /// <param name="outputDirectory">生成目录。</param>
        /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="buildReportPath">生成报告路径。</param>
        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, string unityVersion,
            string applicableGameVersion,
            int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected,
            bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
            bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath,
            string buildReportPath)
        {
            mVersionInfo.ForceUpdateApp = GameBuildSettings.GetInstance().ForceUpdateApp;
            mVersionInfo.LatestAppVersion = applicableGameVersion;
            mVersionInfo.AppUpdateUri = GameBuildSettings.GetInstance().AppUpdateUri;
            mVersionInfo.AppUpdateDesc = GameBuildSettings.GetInstance().AppUpdateDesc;

            mVersionInfo.ApplicableGameVersion = GameBuildSettings.GetInstance().ApplicableGameVersion;
            mVersionInfo.InternalResourceVersion = internalResourceVersion;

            GameBuilderExtension.RemoveStreamingAssetsBundles();
        }

        /// <summary>
        /// 某个平台生成开始前的预处理事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected,
            string outputFullPath,
            bool outputPackedSelected, string outputPackedPath)
        {
        }

        /// <summary>
        /// 某个平台生成 AssetBundle 完成事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="assetBundleManifest">AssetBundle 的描述文件。</param>
        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected,
            string outputFullPath,
            bool outputPackedSelected, string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
        }

        /// <summary>
        /// 某个平台可更新模式版本列表文件的输出事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="versionListPath">可更新模式版本列表文件的路径。</param>
        /// <param name="versionListLength">可更新模式版本列表文件的长度。</param>
        /// <param name="versionListHashCode">可更新模式版本列表文件的校验值。</param>
        /// <param name="versionListCompressedLength">可更新模式版本列表文件压缩后的长度。</param>
        /// <param name="versionListCompressedHashCode">可更新模式版本列表文件压缩后的校验值。</param>
        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength, int versionListHashCode, int versionListCompressedLength,
            int versionListCompressedHashCode)
        {
            mVersionInfo.VersionListLength = versionListLength;
            mVersionInfo.VersionListHashCode = versionListHashCode;
            mVersionInfo.VersionListCompressedLength = versionListCompressedLength;
            mVersionInfo.VersionListCompressedHashCode = versionListCompressedHashCode;
        }

        /// <summary>
        /// 某个平台生成结束后的后处理事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="isSuccess">是否生成成功。</param>
        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected,
            string outputFullPath,
            bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            var outputPath = Application.streamingAssetsPath;
            if (outputPackageSelected)
            {
                if (FolderUtil.CopyFilesToRootPath(outputPackagePath, outputPath))
                {
                    Debug.Log($"Copy package files to '{outputPath}' success.");
                }
            }

            if (!outputPackageSelected && outputPackedSelected)
            {
                if (FolderUtil.CopyFilesToRootPath(outputPackedPath, outputPath))
                {
                    Debug.Log($"Copy packed files to '{outputPath}' success.");
                }
            }

            if (outputFullSelected)
            {
                mVersionInfo.UpdatePrefixUri = PathUtil.GetCombinePath(GameBuildSettings.GetInstance().UpdatePrefixUri, platform.ToString());

                var versionInfoJson = JsonConvert.SerializeObject(mVersionInfo);
                Builtin.FileUtil.CreateFile(Path.Combine(outputFullPath, GameBuildSettings.GetInstance().ResourceVersionFileName), versionInfoJson);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 所有平台生成结束后的后处理事件。
        /// </summary>
        /// <param name="productName">产品名称。</param>
        /// <param name="companyName">公司名称。</param>
        /// <param name="gameIdentifier">游戏识别号。</param>
        /// <param name="gameFrameworkVersion">游戏框架版本。</param>
        /// <param name="unityVersion">Unity 版本。</param>
        /// <param name="applicableGameVersion">适用游戏版本。</param>
        /// <param name="internalResourceVersion">内部资源版本。</param>
        /// <param name="platforms">生成的目标平台。</param>
        /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
        /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
        /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
        /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
        /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
        /// <param name="outputDirectory">生成目录。</param>
        /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="buildReportPath">生成报告路径。</param>
        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, string unityVersion,
            string applicableGameVersion,
            int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected,
            bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
            bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath,
            string buildReportPath)
        {
        }
    }
}