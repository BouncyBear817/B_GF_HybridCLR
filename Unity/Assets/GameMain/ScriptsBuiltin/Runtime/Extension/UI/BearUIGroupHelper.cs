// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/4/19 15:14:48
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    /// <summary>
    /// UGUI界面组辅助器
    /// </summary>
    public class BearUIGroupHelper : UIGroupHelperBase
    {
        public const int DepthFactor = 1000;

        private int mDepth = 0;
        private Canvas mCachedCanvas = null;

        /// <summary>
        /// 设置界面组深度
        /// </summary>
        /// <param name="depth">界面组深度</param>
        public override void SetDepth(int depth)
        {
            mDepth = depth;
            mCachedCanvas.overrideSorting = true;
            mCachedCanvas.sortingOrder = DepthFactor * depth;
        }

        private void Awake()
        {
            mCachedCanvas = gameObject.GetOrAddComponent<Canvas>();

            mCachedCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            mCachedCanvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            mCachedCanvas.planeDistance = 100f;

            var canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280, 720);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            mCachedCanvas.overrideSorting = true;
            mCachedCanvas.sortingOrder = DepthFactor * mDepth;
            this.transform.localPosition = Vector3.zero;

            var trans = GetComponent<RectTransform>();
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.anchoredPosition = Vector2.zero;
            trans.sizeDelta = Vector2.zero;
        }
    }
}