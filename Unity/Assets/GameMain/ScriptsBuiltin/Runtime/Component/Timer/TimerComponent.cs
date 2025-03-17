using System;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public class TimerComponent : GameFrameworkComponent
    {
        private ITimerManager mTimerManager;

        protected override void Awake()
        {
            base.Awake();

            mTimerManager = new TimerManager();
        }

        private void Update()
        {
            mTimerManager.Update();
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
            return mTimerManager.AddOnceTimer(time, callback, updateCallback);
        }

        /// <summary>
        /// 可等待执行一次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <returns></returns>
        public async UniTask<bool> AddOnceTimerAsync(long time)
        {
            return await mTimerManager.AddOnceTimerAsync(time);
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
            return mTimerManager.AddRepeatedTimer(time, repeatCount, callback, updateCallback);
        }

        /// <summary>
        /// 是否存在计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns></returns>
        public bool IsExistTimer(int id)
        {
            return mTimerManager.IsExistTimer(id);
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void PauseTimer(int id)
        {
            mTimerManager.PauseTimer(id);
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void ResumeTimer(int id)
        {
            mTimerManager.ResumeTimer(id);
        }

        /// <summary>
        /// 修改计时器时间
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <param name="time">修改的时间</param>
        /// <param name="isChangeRepeat">是否修改Repeated类型下的计时器</param>
        public void ChangeTimer(int id, long time, bool isChangeRepeat = false)
        {
            mTimerManager.ChangeTimer(id, time, isChangeRepeat);
        }

        /// <summary>
        /// 取消计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void CancelTimer(int id)
        {
            mTimerManager.CancelTimer(id);
        }
    }
}