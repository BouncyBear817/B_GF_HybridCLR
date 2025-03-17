using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework;

namespace GameMain.Hotfix
{
    /// <summary>
    /// Await Result 包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AwaitResultWrap<T> : IReference
    {
        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 任务完成来源
        /// </summary>
        public UniTaskCompletionSource<T> Source { get; private set; }

        public static AwaitResultWrap<T> Create(object userData, UniTaskCompletionSource<T> source)
        {
            var result = ReferencePool.Acquire<AwaitResultWrap<T>>();
            result.UserData = userData;
            result.Source = source;
            return result;
        }

        public void Clear()
        {
            UserData = null;
            Source = null;
        }
    }
}