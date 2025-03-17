// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/9 15:35:39
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework.UI;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace GameMain.Builtin
{
    public class BearUIFormHelper : UIFormHelperBase
    {
        private ResourceComponent mResourceComponent;

        public override object InstantiateUIForm(object uiFormAsset)
        {
            return Instantiate((Object)uiFormAsset);
        }

        public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            var obj = uiFormInstance as GameObject;
            if (obj == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            var trans = obj.transform;
            trans.SetParent(((MonoBehaviour)uiGroup.Helper).transform);
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            return obj.GetOrAddComponent<UIForm>();
        }

        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            mResourceComponent.UnloadAsset(uiFormAsset);
            Destroy((Object)uiFormInstance);
        }

        private void Start()
        {
            mResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (mResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
            }
            
            transform.SetAsFirstSibling();
        }
    }
}