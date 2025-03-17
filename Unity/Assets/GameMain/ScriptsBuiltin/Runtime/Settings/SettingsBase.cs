using System.Threading.Tasks;
using UnityEngine;

namespace GameMain.Builtin
{
    public abstract class SettingsBase<T> : ScriptableObject where T : ScriptableObject, new()
    {
        protected static T mInstance;

        public static T GetInstance()
        {
            if (mInstance == null)
            {
#if UNITY_EDITOR
                mInstance = SettingsUtils.GetSettings<T>();
#else
                var asset = AssetUtil.GetSettingsResourcesAsset(typeof(T).Name);
                mInstance = SettingsUtils.GetSettingsByResources<T>(asset);
#endif
            }

            return mInstance;
        }

    }
}