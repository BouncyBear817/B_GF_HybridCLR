// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/10 11:19:11
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public class MessengerManager : IMessengerManager
    {
        private readonly GameFrameworkMultiDictionary<uint, MessageEvent> mMessengerMap = new GameFrameworkMultiDictionary<uint, MessageEvent>();

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="args">消息参数</param>
        public void Broadcast(uint msgId, params object[] args)
        {
            if (!mMessengerMap.TryGetValue(msgId, out var messageEvents))
            {
                Log.Warning($"Broadcast failed, the msg Id '{msgId}' has no message event.");
                return;
            }

            foreach (var messageEvent in messageEvents)
            {
                messageEvent?.Invoke(args);
            }
        }

        /// <summary>
        /// 增加消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        public void AddListener(uint msgId, MessageEvent messageEvent)
        {
            if (mMessengerMap.Contains(msgId, messageEvent))
            {
                Log.Warning($"Add Listener failed, the msg Id '{msgId}' has already been added to this message event.");
                return;
            }

            mMessengerMap.Add(msgId, messageEvent);
        }

        /// <summary>
        /// 移除消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        public void RemoveListener(uint msgId, MessageEvent messageEvent)
        {
            if (!mMessengerMap.Contains(msgId, messageEvent))
            {
                Log.Warning($"Removed Listener failed, the msg Id '{msgId}' does not contain this message event.");
                return;
            }

            mMessengerMap.Remove(msgId, messageEvent);
        }

        /// <summary>
        /// 移除消息Id下的所有消息事件
        /// </summary>
        /// <param name="msgId">消息Id</param>
        public void RemoveAll(uint msgId)
        {
            mMessengerMap.RemoveAll(msgId);
        }
    }
}