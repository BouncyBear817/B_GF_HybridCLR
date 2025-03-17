// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/5 16:35:3
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.Threading.Tasks;
using UnityEngine;

namespace GameMain.Builtin
{
    public static class SettingsUtils
    {
       

#if UNITY_EDITOR
        public static T GetSettings<T>() where T : ScriptableObject, new()
        {
            var assetType = typeof(T).Name;
            var paths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
            if (paths.Length == 0)
            {
                Debug.LogError($"{assetType} is not existed.");
                return null;
            }

            if (paths.Length > 1)
            {
                Debug.LogError($"{assetType} is more than 1, please delete others and leave one.");
                return null;
            }

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(paths[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static ScriptableObject GetSettings(string assetTypeName)
        {
            var paths = UnityEditor.AssetDatabase.FindAssets($"t:{assetTypeName}");
            if (paths.Length == 0)
            {
                Debug.LogError($"{assetTypeName} is not existed.");
                return null;
            }

            if (paths.Length > 1)
            {
                Debug.LogError($"{assetTypeName} is more than 1, please delete others and leave one.");
                return null;
            }

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(paths[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
        }
#endif

        public static T GetSettingsByResources<T>(string assetsPath) where T : ScriptableObject, new()
        {
            var settings = Resources.Load<T>(assetsPath);
            if (settings == null)
            {
                Debug.LogError($"Not found {typeof(T).Name} asset, please create one.");
                return null;
            }

            return settings;
        }
    }
}