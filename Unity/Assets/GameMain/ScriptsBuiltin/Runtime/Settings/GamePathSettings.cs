// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/09 11:10:56
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameMain.Builtin;
using UnityEngine;

namespace GameMain.Builtin
{
    [CreateAssetMenu(fileName = "GamePathSettings", menuName = "Tools/Game Path Settings", order = 3)]
    public class GamePathSettings : SettingsBase<GamePathSettings>
    {
        [SerializeField] private string mConfigPath;
        [SerializeField] private string mDataTablePath;
        [SerializeField] private string mLocalizationPath;

        [SerializeField] private string mConfigExcelPath;
        [SerializeField] private string mDataTableExcelPath;
        [SerializeField] private string mLocalizationExcelPath;

        [SerializeField] private string mEntityGroupDataTableExcelPath;
        [SerializeField] private string mSoundGroupDataTableExcelPath;
        [SerializeField] private string mUIGroupDataTableExcelPath;

        [SerializeField] private string mDataTableGroupCodePath;

        [SerializeField] private string mEntityPath;
        [SerializeField] private string mFontPath;
        [SerializeField] private string mMusicPath;
        [SerializeField] private string mScenePath;
        [SerializeField] private string mSoundPath;

        [SerializeField] private string mUIPath;


        public string ConfigPath
        {
            get => mConfigPath;
            set => mConfigPath = value;
        }

        public string DataTablePath
        {
            get => mDataTablePath;
            set => mDataTablePath = value;
        }

        public string LocalizationPath
        {
            get => mLocalizationPath;
            set => mLocalizationPath = value;
        }

        public string ConfigExcelPath
        {
            get => mConfigExcelPath;
            set => mConfigExcelPath = value;
        }

        public string DataTableExcelPath
        {
            get => mDataTableExcelPath;
            set => mDataTableExcelPath = value;
        }

        public string LocalizationExcelPath
        {
            get => mLocalizationExcelPath;
            set => mLocalizationExcelPath = value;
        }

        public string EntityGroupDataTableExcelPath
        {
            get => mEntityGroupDataTableExcelPath;
            set => mEntityGroupDataTableExcelPath = value;
        }

        public string SoundGroupDataTableExcelPath
        {
            get => mSoundGroupDataTableExcelPath;
            set => mSoundGroupDataTableExcelPath = value;
        }

        public string UIGroupDataTableExcelPath
        {
            get => mUIGroupDataTableExcelPath;
            set => mUIGroupDataTableExcelPath = value;
        }

        public string DataTableGroupCodePath
        {
            get => mDataTableGroupCodePath;
            set => mDataTableGroupCodePath = value;
        }

        public string EntityPath
        {
            get => mEntityPath;
            set => mEntityPath = value;
        }

        public string FontPath
        {
            get => mFontPath;
            set => mFontPath = value;
        }

        public string MusicPath
        {
            get => mMusicPath;
            set => mMusicPath = value;
        }

        public string ScenePath
        {
            get => mScenePath;
            set => mScenePath = value;
        }

        public string SoundPath
        {
            get => mSoundPath;
            set => mSoundPath = value;
        }

        public string UIPath
        {
            get => mUIPath;
            set => mUIPath = value;
        }
    }
}