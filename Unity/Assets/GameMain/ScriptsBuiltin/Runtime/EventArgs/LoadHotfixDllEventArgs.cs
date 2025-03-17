using GameFramework;
using GameFramework.Event;

namespace GameMain.Builtin
{
    public class LoadHotfixDllEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LoadHotfixDllEventArgs).GetHashCode();
        public override int Id => EventId;

        public string DllName { get; private set; }

        public System.Reflection.Assembly Assembly { get; private set; }

        public object UserData { get; private set; }

        public static LoadHotfixDllEventArgs Create(string dllName, System.Reflection.Assembly assembly, object userData)
        {
            var eventArgs = ReferencePool.Acquire<LoadHotfixDllEventArgs>();
            eventArgs.DllName = dllName;
            eventArgs.Assembly = assembly;
            eventArgs.UserData = userData;
            return eventArgs;
        }

        public override void Clear()
        {
            DllName = default;
            Assembly = null;
            UserData = null;
        }
    }
}