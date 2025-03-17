/************************************************************
 * Unity Version: 2022.3.15f1c1
 * Author:        bear
 * CreateTime:    2024/02/02 16:46:19
 * Description:
 * Modify Record:
 *************************************************************/

using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public sealed partial class MainEntry : MonoBehaviour
    {
        public static NetConnectorComponent NetConnector { get; private set; }

        public static MessengerComponent Messenger { get; private set; }

        public static CrossPlatformComponent CrossPlatform { get; private set; }

        public static BuiltinUIFormComponent BuiltinUIForm { get; private set; }
        
        public static TimerComponent Timer { get; private set; }

        private static void InitCustomComponents()
        {
            NetConnector = GameEntry.GetComponent<NetConnectorComponent>();

            Messenger = GameEntry.GetComponent<MessengerComponent>();

            CrossPlatform = GameEntry.GetComponent<CrossPlatformComponent>();

            BuiltinUIForm = GameEntry.GetComponent<BuiltinUIFormComponent>();

            Timer = GameEntry.GetComponent<TimerComponent>();
        }
    }
}