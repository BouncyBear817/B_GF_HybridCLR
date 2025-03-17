using GameFramework;

namespace GameMain.Hotfix
{
    /// <summary>
    /// Web Request Result
    /// </summary>
    public class WebRequestResult : IReference
    {
        /// <summary>
        /// Web 请求数据
        /// </summary>
        public byte[] Bytes { get; private set; }

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

        public static WebRequestResult Create(byte[] bytes, bool isError, string errorMessage, object userData)
        {
            var result = ReferencePool.Acquire<WebRequestResult>();
            result.Bytes = bytes;
            result.IsError = isError;
            result.ErrorMessage = errorMessage;
            result.UserData = userData;
            return result;
        }

        public void Clear()
        {
            Bytes = null;
            IsError = false;
            ErrorMessage = null;
            UserData = null;
        }
    }
}