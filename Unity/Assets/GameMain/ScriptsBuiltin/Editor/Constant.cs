using System.IO;
using GameMain.Builtin;
using UnityEngine.Device;

namespace GameMain.Editor
{
    public class Constant
    {
        public const string BuiltinAssembly = "Assets/GameMain/ScriptsBuiltin/Runtime/GameMain.Builtin.asmdef";
        public const string HotfixAssembly = "Assets/GameMain/ScriptsHotfix/Hotfix.asmdef";

        public const string HotfixDllDir = "GameMain/HotfixDlls";
        public const string HotfixListFile = "GameMain/HotfixDlls/HotfixDllList.txt";
        public const string AotDlls = "GameMain/Resources/AotDlls";

        public const string SharedAssetBundleName = "SharedAssets";

        public const string UITableExcel = "UITable.xlsx";
        public const string UIViewScriptFile = "Assets/GameMain/ScriptsHotfix/UI/UIViews.cs";
        
        public static string AssetBundleOutputPath => PathUtil.GetCombinePath(Directory.GetParent(Application.dataPath)?.FullName, "GameAssetBundle");
        
    }
}