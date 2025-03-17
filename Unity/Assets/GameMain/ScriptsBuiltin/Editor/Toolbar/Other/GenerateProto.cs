using System.IO;
using UnityEngine;

namespace GameMain.Editor
{
    public static class GenerateProto
    {
        [ToolsMenuMethod("Other/Generator/Proto To Script", null, 40, 1)]
        public static void Generator()
        {
#if UNITY_EDITOR_WIN
            Application.OpenURL(Path.Combine(Application.dataPath, "../../Tools/protoc-25.3/Gen_Proto.bat"));
#else
#endif
        }
    }
}