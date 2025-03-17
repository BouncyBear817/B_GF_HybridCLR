using System;

namespace GameMain.Builtin
{
    public static class TimerUtility
    {
        private static readonly long Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// 当前时间戳（毫秒）
        /// </summary>
        /// <returns>当前时间戳（毫秒）</returns>
        public static long Now()
        {
            return (DateTime.UtcNow.Ticks - Epoch) / TimeSpan.TicksPerMillisecond;
        }
    }
}