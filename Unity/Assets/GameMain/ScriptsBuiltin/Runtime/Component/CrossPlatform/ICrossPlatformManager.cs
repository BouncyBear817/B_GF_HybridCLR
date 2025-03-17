// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/12 14:30:7
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    public interface ICrossPlatformManager
    {
        /// <summary>
        /// 调起相机
        /// </summary>
        void HandleCamera();

        /// <summary>
        /// 释放资源
        /// </summary>
        void Dispose();
    }
}