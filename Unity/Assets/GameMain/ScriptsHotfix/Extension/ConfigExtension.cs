// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/16 14:10:46
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameMain.Builtin;
using UnityGameFramework.Runtime;

namespace GameMain.Hotfix
{
    public static class ConfigExtension
    {
        public static void LoadConfig(this ConfigComponent configComponent, string configName, string configAssetName, object userData)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Log.Warning("Config name is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(configAssetName))
            {
                Log.Warning("Config asset name is invalid.");
                return;
            }
            
            configComponent.ReadData(configAssetName, Constant.AssetPriority.ConfigAsset, userData);
        }
    }
}