// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/4/23 11:15:36
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Builtin
{
    [CreateAssetMenu(fileName = "UIAutoBindGlobalSettings", menuName = "Tools/UI Auto Bind Settings", order = 20)]
    public class UIAutoBindGlobalSettings : ScriptableObject
    {
        [SerializeField] private string mNamespace;

        [Header("Bind Code Saved Path")] [SerializeField]
        private string mComponentCodePath;

        [Header("Mount Code Saved Path")] [SerializeField]
        private string mMountCodePath;

        [Header("Mount Code Search Assembly")] [SerializeField]
        private List<string> mMountScriptAssemblyList = new List<string> { };

        [Header("Prefix name mapping for components")] [SerializeField]
        private List<UIAutoBindPrefixRule> mPrefixRuleList = new List<UIAutoBindPrefixRule>()
        {
            new UIAutoBindPrefixRule("B", "Button"),
            new UIAutoBindPrefixRule("I", "Image"),
            new UIAutoBindPrefixRule("RI", "RawImage"),
            new UIAutoBindPrefixRule("TM", "TextMeshProUGUI"),
            new UIAutoBindPrefixRule("In", "TMP_InputField"),
            new UIAutoBindPrefixRule("To", "Toggle"),
            new UIAutoBindPrefixRule("Sl", "Slider"),
            new UIAutoBindPrefixRule("Sb", "Scrollbar"),
            new UIAutoBindPrefixRule("SV", "ScrollRect"),
            new UIAutoBindPrefixRule("Dr", "TMP_Dropdown"),

            new UIAutoBindPrefixRule("Trans", "Transform"),
            new UIAutoBindPrefixRule("OAnim", "Animation"),
            new UIAutoBindPrefixRule("NAnim", "Animator"),
            new UIAutoBindPrefixRule("Rect", "RectTransform"),
            new UIAutoBindPrefixRule("Canvas", "Canvas"),
            new UIAutoBindPrefixRule("CGroup", "CanvasGroup"),
            new UIAutoBindPrefixRule("VLGroup", "VerticalLayoutGroup"),
            new UIAutoBindPrefixRule("HLGroup", "HorizontalLayoutGroup"),
            new UIAutoBindPrefixRule("GLGroup", "GridLayoutGroup"),
            new UIAutoBindPrefixRule("TGroup", "ToggleGroup"),
            new UIAutoBindPrefixRule("Mask", "Mask"),
            new UIAutoBindPrefixRule("Mask2D", "RectMask2D"),
        };

        public string Namespace => mNamespace;

        public string ComponentCodePath => mComponentCodePath;

        public string MountCodePath => mMountCodePath;

        public List<string> MountScriptAssemblyList => mMountScriptAssemblyList;

        public List<UIAutoBindPrefixRule> PrefixRuleList => mPrefixRuleList;

        public static bool IsValidBind(Transform target, List<string> filedNames, List<string> typeNames)
        {
            var strArray = target.name.Split('_');
            if (strArray.Length <= 1)
            {
                return false;
            }

            var autoBindGlobalSetting = SettingsUtils.GetSettings<UIAutoBindGlobalSettings>();
            var prefixRuleList = autoBindGlobalSetting.PrefixRuleList;

            var isValid = false;
            var filedName = strArray[^1];
            filedName = filedName.Replace(" ", "").Replace("(", "").Replace(")", "");
            for (int i = 0; i < strArray.Length - 1; i++)
            {
                var targetPrefix = strArray[i];
                var isFindType = false;
                foreach (var prefixRule in prefixRuleList)
                {
                    if (prefixRule.Prefix.Equals(targetPrefix))
                    {
                        var typeName = prefixRule.FullName;
                        filedNames.Add($"{targetPrefix}{filedName}");
                        typeNames.Add(typeName);
                        isFindType = true;
                        isValid = true;
                        break;
                    }
                }

                if (!isFindType)
                {
                    Debug.LogWarning($"Bind failure, {target.name} does not match type for {targetPrefix}");
                }
            }

            return isValid;
        }
    }
}