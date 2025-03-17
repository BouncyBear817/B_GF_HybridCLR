using System;
using Cysharp.Threading.Tasks;
using GameFramework;

namespace GameMain.Builtin
{
    public partial class TimerManager : ITimerManager
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private class Timer : IReference
        {
            private static int sSerialId;

            static Timer()
            {
                sSerialId = 0;
            }

            /// <summary>
            /// 计时器ID
            /// </summary>
            public int ID { get; private set; }

            /// <summary>
            /// Timer 类型
            /// </summary>
            public TimerType TimerType { get; private set; }

            /// <summary>
            /// 时间
            /// </summary>
            public long Time { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            public long StartTime { get; set; }

            /// <summary>
            /// 重复执行次数
            /// </summary>
            public int RepeatedCount { get; set; }

            /// <summary>
            /// 计时器回调
            /// </summary>
            public object Callback { get; private set; }

            /// <summary>
            /// 计时器每帧调用
            /// </summary>
            public Action<long> UpdateCallback { get; private set; }

            /// <summary>
            /// 创建计时器
            /// </summary>
            /// <param name="timerType">Timer 类型</param>
            /// <param name="time">时间</param>
            /// <param name="startTime">开始时间</param>
            /// <param name="callback">计时器回调</param>
            /// <param name="repeatedCount">重复执行次数</param>
            /// <param name="updateCallback">计时器每帧调用</param>
            /// <returns>计时器</returns>
            public static Timer Create(TimerType timerType, long time, long startTime, object callback, int repeatedCount = 0, Action<long> updateCallback = null)
            {
                var timer = ReferencePool.Acquire<Timer>();
                timer.ID = sSerialId++;
                timer.TimerType = timerType;
                timer.Time = time;
                timer.StartTime = startTime;
                timer.Callback = callback;
                timer.RepeatedCount = repeatedCount;
                timer.UpdateCallback = updateCallback;
                return timer;
            }

            /// <summary>
            /// 清理引用。
            /// </summary>
            public void Clear()
            {
                ID = -1;
                TimerType = TimerType.None;
                Time = 0;
                StartTime = 0;
                Callback = null;
                RepeatedCount = 0;
                UpdateCallback = null;
            }
        }
    }
}