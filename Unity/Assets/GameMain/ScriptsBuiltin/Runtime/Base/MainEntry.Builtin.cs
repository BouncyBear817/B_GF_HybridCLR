/************************************************************
 * Unity Version: 2022.3.15f1c1
 * Author:        bear
 * CreateTime:    2024/01/30 16:12:05
 * Description:
 * Modify Record:
 *************************************************************/

using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public sealed partial class MainEntry : MonoBehaviour
    {
        public static BaseComponent Base { get; private set; }

        public static ConfigComponent Config { get; private set; }

        public static DataNodeComponent DataNode { get; private set; }

        public static DataTableComponent DataTable { get; private set; }

        public static DebuggerComponent Debugger { get; private set; }

        public static DownloadComponent Download { get; private set; }
        
        public static EntityComponent Entity { get; private set; }

        public static EventComponent Event { get; private set; }

        public static FileSystemComponent FileSystem { get; private set; }

        public static FsmComponent Fsm { get; private set; }
        
        public static LocalizationComponent Localization { get; private set; }

        public static NetworkComponent Network { get; private set; }

        public static ObjectPoolComponent ObjectPool { get; private set; }

        public static ProcedureComponent Procedure { get; private set; }

        public static ResourceComponent Resource { get; private set; }

        public static SceneComponent Scene { get; private set; }

        public static SettingComponent Setting { get; private set; }

        public static SoundComponent Sound { get; private set; }

        public static UIComponent UI { get; private set; }

        public static WebRequestComponent WebRequest { get; private set; }

        private static void InitBuiltinComponents()
        {
            Base = GameEntry.GetComponent<BaseComponent>();
            Config = GameEntry.GetComponent<ConfigComponent>();
            DataNode = GameEntry.GetComponent<DataNodeComponent>();
            DataTable = GameEntry.GetComponent<DataTableComponent>();
            Debugger = GameEntry.GetComponent<DebuggerComponent>();
            Download = GameEntry.GetComponent<DownloadComponent>();
            Entity = GameEntry.GetComponent<EntityComponent>();
            Event = GameEntry.GetComponent<EventComponent>();
            FileSystem = GameEntry.GetComponent<FileSystemComponent>();
            Fsm = GameEntry.GetComponent<FsmComponent>();
            Localization = GameEntry.GetComponent<LocalizationComponent>();
            Network = GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = GameEntry.GetComponent<ProcedureComponent>();
            Resource = GameEntry.GetComponent<ResourceComponent>();
            Scene = GameEntry.GetComponent<SceneComponent>();
            Setting = GameEntry.GetComponent<SettingComponent>();
            Sound = GameEntry.GetComponent<SoundComponent>();
            UI = GameEntry.GetComponent<UIComponent>();
            WebRequest = GameEntry.GetComponent<WebRequestComponent>();
        }
    }
}