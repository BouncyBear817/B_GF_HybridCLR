using System;
using Cysharp.Threading.Tasks;

namespace GameMain.Builtin
{
    public interface ITimerManager
    {
        void Update();
        
        /// <summary>
        /// 添加执行一次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <param name="callback">计时器回调</param>
        /// <param name="updateCallback">计时器每帧调用</param>
        /// <returns>计时器ID</returns>
        int AddOnceTimer(long time, Action callback, Action<long> updateCallback = null);

        /// <summary>
        /// 可等待执行一次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <returns></returns>
        UniTask<bool> AddOnceTimerAsync(long time);

        /// <summary>
        /// 添加执行多次的计时器
        /// </summary>
        /// <param name="time">计时时间</param>
        /// <param name="repeatCount">重复次数</param>
        /// <param name="callback">计时器回调</param>
        /// <param name="updateCallback">计时器每帧调用</param>
        /// <returns>计时器ID</returns>
        public int AddRepeatedTimer(long time, int repeatCount, Action callback, Action<long> updateCallback = null);

        /// <summary>
        /// 是否存在计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns></returns>
        public bool IsExistTimer(int id);

        /// <summary>
        /// 暂停计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void PauseTimer(int id);

        /// <summary>
        /// 恢复计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void ResumeTimer(int id);

        /// <summary>
        /// 修改计时器时间
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <param name="time">修改的时间</param>
        /// <param name="isChangeRepeat">是否修改Repeated类型下的计时器</param>
        public void ChangeTimer(int id, long time, bool isChangeRepeat = false);

        /// <summary>
        /// 取消计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void CancelTimer(int id);
    }
}