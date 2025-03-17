// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/14 15:4:15
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    /// <summary>
    /// 跨平台组件
    /// </summary>
    public sealed class CrossPlatformComponent : GameFrameworkComponent
    {
        private ICrossPlatformManager mCrossPlatformManager;

        protected override void Awake()
        {
            base.Awake();

            mCrossPlatformManager =
#if UNITY_ANDROID
                new CrossPlatformManagerAndroid();
#elif UNITY_IOS
                new CrossPlatformManagerIOS();
#else
                new CrossPlatformManager();
#endif
        }

        public void OpenCamera()
        {
            mCrossPlatformManager.HandleCamera();
        }

        public void Dispose()
        {
            mCrossPlatformManager.Dispose();
        }
    }
}