// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/10 14:5:8
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public sealed class MessengerComponent : GameFrameworkComponent
    {
        private IMessengerManager mMessengerManager;

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="args">消息参数</param>
        public void Broadcast(uint msgId, params object[] args)
        {
            mMessengerManager.Broadcast(msgId, args);
        }

        /// <summary>
        /// 增加消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        public void AddListener(uint msgId, MessageEvent messageEvent)
        {
            mMessengerManager.AddListener(msgId, messageEvent);
        }

        /// <summary>
        /// 移除消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        public void RemoveListener(uint msgId, MessageEvent messageEvent)
        {
            mMessengerManager.RemoveListener(msgId, messageEvent);
        }

        /// <summary>
        /// 移除消息Id下的所有消息事件
        /// </summary>
        /// <param name="msgId">消息Id</param>
        public void RemoveAll(uint msgId)
        {
            mMessengerManager.RemoveAll(msgId);
        }

        protected override void Awake()
        {
            base.Awake();
            mMessengerManager = new MessengerManager();
        }
    }
}