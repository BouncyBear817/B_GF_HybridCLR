// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/9 16:11:23
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    /// <summary>
    /// 消息事件
    /// </summary>
    public delegate void MessageEvent(params object[] args);

    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="args">消息参数</param>
        void Broadcast(uint msgId, params object[] args);

        /// <summary>
        /// 增加消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        void AddListener(uint msgId, MessageEvent messageEvent);

        /// <summary>
        /// 移除消息监听器
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <param name="messageEvent">消息事件</param>
        void RemoveListener(uint msgId, MessageEvent messageEvent);
        
        /// <summary>
        /// 移除消息Id下的所有消息事件
        /// </summary>
        /// <param name="msgId">消息Id</param>
        void RemoveAll(uint msgId);
    }
}