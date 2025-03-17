// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/5 15:56:38
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    /// <summary>
    /// 资源服务器类型
    /// </summary>
    public enum ServerType : byte
    {
        None = 0,

        /// <summary>
        /// 内网
        /// </summary>
        InternalNet,

        /// <summary>
        /// 外网
        /// </summary>
        ExternalNet,

        /// <summary>
        /// 正式服
        /// </summary>
        FormalNet
    }
}