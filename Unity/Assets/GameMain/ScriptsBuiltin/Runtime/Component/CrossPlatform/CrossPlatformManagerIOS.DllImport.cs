// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/14 14:3:52
//  * Description:
//  * Modify Record:
//  *************************************************************/

#if UNITY_IOS
using System.Runtime.InteropServices;

namespace GameMain.Builtin
{
    public partial class CrossPlatformManagerIOS
    {
        [DllImport("__Internal")]
        private static extern void handleCamera();
    }
}
#endif