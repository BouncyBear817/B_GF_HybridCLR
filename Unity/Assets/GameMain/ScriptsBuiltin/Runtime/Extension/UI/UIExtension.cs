/************************************************************
 * Unity Version: 2022.3.15f1c1
 * Author:        bear
 * CreateTime:    2024/4/19 9:57:29
 * Description:
 * Modify Record:
 *************************************************************/

using System.Collections;
using GameFramework;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    /// <summary>
    /// UI扩展
    /// </summary>
    public static class UIExtension
    {
        private static Transform mInstanceRoot;

        public static Transform InstanceRoot
        {
            get
            {
                if (mInstanceRoot == null)
                {
                    mInstanceRoot = MainEntry.UI.transform.Find("UI Form Instances");
                }

                return mInstanceRoot;
            }
        }

        private static Camera mUICamera;

        public static Camera UICamera
        {
            get
            {
                if (mUICamera == null && MainEntry.UI != null && MainEntry.UI.transform != null)
                {
                    var cameraTransform = MainEntry.UI.transform.Find("UICamera");
                    if (cameraTransform != null)
                    {
                        mUICamera = cameraTransform.GetComponent<Camera>();
                    }
                }

                return mUICamera;
            }
        }

        private static IUIManager mUIManager;

        public static IUIManager UIManager
        {
            get
            {
                if (mUIManager == null)
                {
                    mUIManager = GameFrameworkEntry.GetModule<IUIManager>();
                }

                return mUIManager;
            }
        }

        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            var time = 0f;
            var originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            var time = 0f;
            var originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }

        public static void CloseUIForm(this UIComponent uiComponent, BearUIForm uiForm)
        {
            uiComponent.CloseUIForm(uiForm.UIForm);
        }

        /// <summary>
        /// 适配挖孔屏与刘海屏
        /// </summary>
        /// <param name="topBar"></param>
        public static void FitHoleScreen(RectTransform topBar)
        {
            if (topBar == null)
            {
                return;
            }

            var topSpace = Screen.height - Screen.safeArea.height;
            if (topSpace < 1f)
            {
                return;
            }

#if UNITY_IOS
            topSpace = 80;
#endif
            var pos = topBar.anchoredPosition;
            pos.y = -topSpace;
            topBar.anchoredPosition = pos;
        }

        public static IUIFormHelper GetUIFormHelper(this UIComponent uiComponent)
        {
            return uiComponent.GetComponentInChildren<IUIFormHelper>();
        }

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float anchoredPositionX)
        {
            var value = rectTransform.anchoredPosition;
            value.x = anchoredPositionX;
            rectTransform.anchoredPosition = value;
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float anchoredPositionY)
        {
            var value = rectTransform.anchoredPosition;
            value.y = anchoredPositionY;
            rectTransform.anchoredPosition = value;
        }

        public static void SetAnchoredPosition3DZ(this RectTransform rectTransform, float anchoredPositionZ)
        {
            var value = rectTransform.anchoredPosition3D;
            value.z = anchoredPositionZ;
            rectTransform.anchoredPosition3D = value;
        }

        public static void SetColorAlpha(this UnityEngine.UI.Graphic graphic, float alpha)
        {
            var value = graphic.color;
            value.a = alpha;
            graphic.color = value;
        }

        public static Vector2 GetFlexibleSize(this LayoutElement layoutElement)
        {
            return new Vector2(layoutElement.flexibleWidth, layoutElement.flexibleHeight);
        }

        public static void SetFlexibleSize(this LayoutElement layoutElement, Vector2 flexibleSize)
        {
            layoutElement.flexibleWidth = flexibleSize.x;
            layoutElement.flexibleHeight = flexibleSize.y;
        }

        public static Vector2 GetMinSize(this LayoutElement layoutElement)
        {
            return new Vector2(layoutElement.minWidth, layoutElement.minHeight);
        }
        
        public static void SetMinSize(this LayoutElement layoutElement, Vector2 size)
        {
            layoutElement.minWidth = size.x;
            layoutElement.minHeight = size.y;
        }
        
        public static Vector2 GetPreferredSize(this LayoutElement layoutElement)
        {
            return new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
        }
        
        public static void SetPreferredSize(this LayoutElement layoutElement, Vector2 size)
        {
            layoutElement.preferredWidth = size.x;
            layoutElement.preferredHeight = size.y;
        }
    }
}