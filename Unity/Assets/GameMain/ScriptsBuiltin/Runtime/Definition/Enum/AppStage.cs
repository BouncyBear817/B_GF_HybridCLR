// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/5 16:19:6
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    /// <summary>
    /// 应用程序阶段
    /// </summary>
    public enum AppStage : byte
    {
        None = 0,
        /// <summary>
        /// 开发阶段
        /// </summary>
        Development,
        /// <summary>
        /// 调试阶段
        /// </summary>
        Debug,
        /// <summary>
        /// 公开测试阶段
        /// </summary>
        Beta,
        /// <summary>
        /// 发布阶段
        /// </summary>
        Release
    }
}