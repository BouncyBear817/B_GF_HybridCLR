// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/09/20 10:09:08
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.IO;
using GameFramework;
using UnityEngine;

namespace GameMain.Builtin
{
    public static class AssetUtil
    {
        public static string GetConfigAsset(string assetName, bool fromBytes, bool txtOrXml)
        {
            return Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : txtOrXml ? "txt" : "xml");
        }

        public static string GetLocalizationAsset(string assetName, bool bytesOrXml)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}", assetName, MainEntry.Localization.Language, bytesOrXml ? "bytes" : "xml");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes, bool txtOrCsv)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : txtOrCsv ? "txt" : "csv");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}.prefab", assetName);
        }

        public static string GetUITextureAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UITextures/{0}.png", assetName);
        }

        public static string GetSettingsAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Settings/{0}.asset", assetName);
        }

        public static string GetHotfixDllAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/HotfixDlls/{0}.bytes", assetName);
        }

        public static string GetSettingsResourcesAsset(string assetName)
        {
            return Utility.Text.Format("Settings/{0}", assetName);
        }
    }
}