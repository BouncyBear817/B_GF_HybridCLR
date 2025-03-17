using System;
using Cysharp.Threading.Tasks;
using GameFramework;

namespace GameMain.Builtin
{
    public partial class TimerManager : ITimerManager
    {
        /// <summary>
        /// 暂停的计时器
        /// </summary>
        private class PausedTimer : IReference
        {
            /// <summary>
            /// 被暂停的计时器
            /// </summary>
            public Timer Timer { get; private set; }

            /// <summary>
            /// 暂停时间
            /// </summary>
            public long PausedTime { get; private set; }

            /// <summary>
            /// 剩余的时间
            /// </summary>
            public long RemainingTime => Timer.Time + Timer.StartTime - PausedTime;

            /// <summary>
            /// 创建暂停的计时器
            /// </summary>
            /// <param name="pausedTimer">被暂停的计时器</param>
            /// <param name="pausedTime">暂停时间</param>
            /// <returns>暂停的计时器</returns>
            public static PausedTimer Create(Timer pausedTimer, long pausedTime)
            {
                var timer = ReferencePool.Acquire<PausedTimer>();
                timer.Timer = pausedTimer;
                timer.PausedTime = pausedTime;
                return timer;
            }

            /// <summary>
            /// 清理引用。
            /// </summary>
            public void Clear()
            {
                Timer = null;
                PausedTime = 0;
            }
        }
    }
}