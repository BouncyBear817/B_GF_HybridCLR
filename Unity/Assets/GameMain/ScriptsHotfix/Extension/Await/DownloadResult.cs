using GameFramework;

namespace GameMain.Hotfix
{
    /// <summary>
    /// Download Result
    /// </summary>
    public class DownloadResult : IReference
    {
        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public static DownloadResult Create(bool isError, string errorMessage, object userData)
        {
            var result = ReferencePool.Acquire<DownloadResult>();
            result.IsError = isError;
            result.ErrorMessage = errorMessage;
            result.UserData = userData;
            return result;
        }

        public void Clear()
        {
            IsError = false;
            ErrorMessage = null;
            UserData = null;
        }
    }
}