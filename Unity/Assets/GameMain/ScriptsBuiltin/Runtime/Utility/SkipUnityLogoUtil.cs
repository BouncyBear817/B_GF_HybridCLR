// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/08 11:10:07
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace GameMain.Builtin
{
    /// <summary>
    /// 跳过Unity logo splash
    /// </summary>
    [Preserve]
    public class SkipUnityLogoUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
#if UNITY_WEBGL
            Application.focusChanged += Application_focusChanged;
#else
            System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
        }
#if UNITY_WEBGL
        private static void Application_focusChanged(bool obj)
        {
            Application.focusChanged -= Application_focusChanged;
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#else
        private static void AsyncSkip()
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#endif
    }
}