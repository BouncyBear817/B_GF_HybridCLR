// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/09 10:10:52
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.Threading.Tasks;
using GameMain.Builtin;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Hotfix
{
    [CreateAssetMenu(fileName = "GameConfigSettings", menuName = "Tools/Game Config Settings[配置数据表、配置表]", order = 2)]
    public class GameConfigSettings : SettingsBase<GameConfigSettings>
    {
        [SerializeField] private string[] mProcedures;
        [SerializeField] private string[] mDataTables;
        [SerializeField] private string[] mConfigs;
        [SerializeField] private string[] mLocalizations;

        public static async Task<GameConfigSettings> GetInstanceByTask(string assetsPath)
        {
            var asset = AssetUtil.GetSettingsAsset(assetsPath);
            if (mInstance == null)
            {
                mInstance = await MainEntry.Resource.LoadAssetsAsync<GameConfigSettings>(asset);
            }

            return mInstance;
        }
        
#if !UNITY_EDITOR
        public new static GameConfigSettings GetInstance()
        {
            if (mInstance == null)
            {
                Log.Error("Please execute 'GetInstanceByTask(string assetsPath)' first.");
                return null;
            }

            return mInstance;
        }
#endif
        
        public string[] Procedures
        {
            get => mProcedures;
            set => mProcedures = value;
        }

        public string[] DataTables
        {
            get => mDataTables;
            set => mDataTables = value;
        }

        public string[] Configs
        {
            get => mConfigs;
            set => mConfigs = value;
        }

        public string[] Localizations
        {
            get => mLocalizations;
            set => mLocalizations = value;
        }
    }
}