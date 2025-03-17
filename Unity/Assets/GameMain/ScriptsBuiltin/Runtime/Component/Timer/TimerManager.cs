using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public partial class TimerManager : ITimerManager
    {
        private readonly Dictionary<int, Timer> mTimers = new Dictionary<int, Timer>();
        private readonly GameFrameworkMultiDictionary<long, int> mTimerIDs = new GameFrameworkMultiDictionary<long, int>();
        private readonly Queue<long> mTimeOutTimers = new Queue<long>();
        private readonly Queue<int> mTimeoutTimerIds = new Queue<int>();
        private readonly Dictionary<int, PausedTimer> mPausedTimers = new Dictionary<int, PausedTimer>();
        private readonly Dictionary<int, Timer> mUpdateTimers = new Dictionary<int, Timer>();
        private long mMinTime;

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        public void Update()
        {
            RunUpdateCallback();
            if (mTimerIDs.Count == 0)
            {
                return;
            }

            var timeNow = TimerUtility.Now();
            if (timeNow < mMinTime)
            {
                return;
            }

            foreach (var (key, _) in mTimerIDs)
            {
                if (key > timeNow)
                {
                    mMinTime = key;
                    break;
                }

                mTimeOutTimers.Enqueue(key);
            }

            while (mTimeOutTimers.Count > 0)
            {
                var time = mTimeOutTimers.Dequeue();
                foreach (var id in mTimerIDs[time])
                {
                    mTimeoutTimerIds.Enqueue(id);
                }

                mTimerIDs.RemoveAll(time);
            }

            while (mTimeoutTimerIds.Count > 0)
            {
                var timerId = mTimeoutTimerIds.Dequeue();
                if (mTimers.TryGetValue(timerId, out var timer))
                {
                    RunTimerCallback(timer);
                }
            }
        }

        /// <summary>
        /// 添加执行一次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <param name="callback">计时器回调</param>
        /// <param name="updateCallback">计时器每帧调用</param>
        /// <returns>计时器ID</returns>
        public int AddOnceTimer(long time, Action callback, Action<long> updateCallback = null)
        {
            if (time < 0)
            {
                Log.Error("Add once timer failed, time is too small.");
                return -1;
            }

            var nowTime = TimerUtility.Now();
            var timer = Timer.Create(TimerType.Once, time, nowTime, callback, 1, updateCallback);
            mTimers.Add(timer.ID, timer);
            if (updateCallback != null)
            {
                mUpdateTimers.Add(timer.ID, timer);
            }

            AddTimer(nowTime + time, timer.ID);
            return timer.ID;
        }

        /// <summary>
        /// 可等待执行一次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <returns></returns>
        public async UniTask<bool> AddOnceTimerAsync(long time)
        {
            if (time < 0)
            {
                Log.Error("Add once timer failed, time is too small.");
                return true;
            }

            var nowTime = TimerUtility.Now();
            var tcs = new UniTaskCompletionSource<bool>();
            var timer = Timer.Create(TimerType.OnceWait, time, nowTime, tcs);
            mTimers.Add(timer.ID, timer);
            AddTimer(nowTime + time, timer.ID);

            bool result;
            try
            {
                result = await tcs.Task;
            }
            finally
            {
                RemoveTimer(timer.ID);
                tcs.TrySetResult(false);
            }

            return result;
        }

        /// <summary>
        /// 添加执行多次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <param name="repeatCount">重复次数</param>
        /// <param name="callback">计时器回调</param>
        /// <param name="updateCallback">计时器每帧调用</param>
        /// <returns>计时器ID</returns>
        public int AddRepeatedTimer(long time, int repeatCount, Action callback, Action<long> updateCallback = null)
        {
            if (time < 0)
            {
                Log.Error("Add repeated timer failed, time is too small.");
                return -1;
            }

            var nowTime = TimerUtility.Now();
            var timer = Timer.Create(TimerType.Repeated, time, nowTime, callback, repeatCount, updateCallback);
            mTimers.Add(timer.ID, timer);
            if (updateCallback != null)
            {
                mUpdateTimers.Add(timer.ID, timer);
            }

            AddTimer(nowTime + time, timer.ID);
            return timer.ID;
        }

        /// <summary>
        /// 是否存在计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns></returns>
        public bool IsExistTimer(int id)
        {
            return mTimers.ContainsKey(id) || mPausedTimers.ContainsKey(id);
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void PauseTimer(int id)
        {
            if (mTimers.TryGetValue(id, out var timer))
            {
                mTimerIDs.Remove(timer.StartTime + timer.Time, timer.ID);
                mTimers.Remove(id);
                mUpdateTimers.Remove(id);
                var pausedTime = PausedTimer.Create(timer, TimerUtility.Now());
                mPausedTimers.Add(id, pausedTime);
            }
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void ResumeTimer(int id)
        {
            if (mPausedTimers.TryGetValue(id, out var pausedTimer))
            {
                mTimers.Add(id, pausedTimer.Timer);
                if (pausedTimer.Timer.UpdateCallback != null)
                {
                    mUpdateTimers.Add(id, pausedTimer.Timer);
                }

                var tillTime = TimerUtility.Now() + pausedTimer.RemainingTime;
                pausedTimer.Timer.StartTime += TimerUtility.Now() - pausedTimer.PausedTime;
                AddTimer(tillTime, pausedTimer.Timer.ID);
                ReferencePool.Release(pausedTimer);
                mPausedTimers.Remove(id);
            }
        }

        /// <summary>
        /// 修改计时器时间
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <param name="time">修改的时间</param>
        /// <param name="isChangeRepeat">是否修改Repeated类型下的计时器</param>
        public void ChangeTimer(int id, long time, bool isChangeRepeat = false)
        {
            if (mPausedTimers.TryGetValue(id, out var pausedTimer))
            {
                pausedTimer.Timer.Time += time;
                return;
            }

            if (mTimers.TryGetValue(id, out var timer))
            {
                mTimerIDs.Remove(timer.StartTime + timer.Time, timer.ID);
                if (timer.TimerType == TimerType.Repeated && !isChangeRepeat)
                {
                    timer.StartTime += time;
                }
                else
                {
                    timer.Time += time;
                }

                AddTimer(timer.StartTime + timer.Time, timer.ID);
            }
        }

        /// <summary>
        /// 取消计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void CancelTimer(int id)
        {
            if (mPausedTimers.ContainsKey(id))
            {
                ReferencePool.Release(mPausedTimers[id].Timer);
                ReferencePool.Release(mPausedTimers[id]);
                mPausedTimers.Remove(id);
                return;
            }

            RemoveTimer(id);
        }

        private void RunUpdateCallback()
        {
            if (mUpdateTimers.Count == 0)
            {
                return;
            }

            var timeNow = TimerUtility.Now();
            foreach (var (_, timer) in mUpdateTimers)
            {
                timer.UpdateCallback?.Invoke(timer.Time + timer.StartTime - timeNow);
            }
        }

        private void RunTimerCallback(Timer timer)
        {
            switch (timer.TimerType)
            {
                case TimerType.OnceWait:
                {
                    var tcs = timer.Callback as UniTaskCompletionSource<bool>;
                    RemoveTimer(timer.ID);
                    tcs?.TrySetResult(true);
                }
                    break;
                case TimerType.Once:
                {
                    var action = timer.Callback as Action;
                    RemoveTimer(timer.ID);
                    action?.Invoke();
                }
                    break;
                case TimerType.Repeated:
                {
                    var action = timer.Callback as Action;
                    var nowTime = TimerUtility.Now();
                    var tillTime = nowTime + timer.Time;
                    if (timer.RepeatedCount == 1)
                    {
                        RemoveTimer(timer.ID);
                    }
                    else
                    {
                        if (timer.RepeatedCount > 1)
                        {
                            timer.RepeatedCount--;
                        }

                        timer.StartTime = nowTime;
                        AddTimer(tillTime, timer.ID);
                    }

                    action?.Invoke();
                }
                    break;
            }
        }

        private void AddTimer(long tillTime, int id)
        {
            mTimerIDs.Add(tillTime, id);
            if (tillTime < mMinTime)
            {
                mMinTime = tillTime;
            }
        }

        private void RemoveTimer(int id)
        {
            if (mTimers.TryGetValue(id, out var timer))
            {
                ReferencePool.Release(timer);
                mTimers.Remove(id);
                mUpdateTimers.Remove(id);
                if (mPausedTimers.ContainsKey(id))
                {
                    ReferencePool.Release(mPausedTimers[id]);
                    mPausedTimers.Remove(id);
                }
            }
        }
    }
}