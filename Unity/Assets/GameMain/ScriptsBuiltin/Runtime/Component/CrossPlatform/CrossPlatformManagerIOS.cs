// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/14 14:3:52
//  * Description:
//  * Modify Record:
//  *************************************************************/

#if UNITY_IOS
namespace GameMain.Builtin
{
    public partial class CrossPlatformManagerIOS : ICrossPlatformManager
    {
        /// <summary>
        /// 调起相机
        /// </summary>
        public void HandleCamera()
        {
            handleCamera();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
        }
    }
}
#endif