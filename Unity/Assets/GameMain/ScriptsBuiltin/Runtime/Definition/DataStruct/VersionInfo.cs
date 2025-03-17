// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/8/20 14:40:29
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    /// <summary>
    /// 版本信息内容
    /// </summary>
    [System.Serializable]
    public class VersionInfo
    {
        /// <summary>
        /// 是否强制更新App
        /// </summary>
        public bool ForceUpdateApp { get; set; }
        
        /// <summary>
        /// 最新App版本号
        /// </summary>
        public string LatestAppVersion { get; set; }
        
        /// <summary>
        /// App更新下载地址
        /// </summary>
        public string AppUpdateUri { get; set; }
        
        /// <summary>
        /// App更新说明
        /// </summary>
        public string AppUpdateDesc { get; set; }

        /// <summary>
        /// 资源更新下载地址
        /// </summary>
        public string UpdatePrefixUri { get; set; }
        
        /// <summary>
        /// 资源适配的版本号
        /// </summary>
        public string ApplicableGameVersion { get; set; }

        /// <summary>
        /// 最新的内部资源版本号
        /// </summary>
        public int InternalResourceVersion { get; set; }
        
        /// <summary>
        /// 资源版本列表长度
        /// </summary>
        public int VersionListLength { get; set; }

        /// <summary>
        /// 资源版本列表哈希值
        /// </summary>
        public int VersionListHashCode { get; set; }

        /// <summary>
        /// 资源版本列表长度
        /// </summary>
        public int VersionListCompressedLength { get; set; }

        /// <summary>
        /// 资源版本列表长度
        /// </summary>
        public int VersionListCompressedHashCode { get; set; }
    }
}