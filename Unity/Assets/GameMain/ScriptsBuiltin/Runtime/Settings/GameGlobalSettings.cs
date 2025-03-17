// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/5 11:28:24
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityEngine;

namespace GameMain.Builtin
{
    [CreateAssetMenu(fileName = "GameGlobalSettings", menuName = "Tools/Game Global Settings", order = 1)]
    public class GameGlobalSettings : SettingsBase<GameGlobalSettings>
    {
        [SerializeField] private string mScriptAuthor = "Default";
        [SerializeField] private Font mMainFont;

        public string ScriptAuthor
        {
            get => mScriptAuthor;
            set => mScriptAuthor = value;
        }

        public Font MainFont
        {
            get => mMainFont;
            set => mMainFont = value;
        }
    }
}