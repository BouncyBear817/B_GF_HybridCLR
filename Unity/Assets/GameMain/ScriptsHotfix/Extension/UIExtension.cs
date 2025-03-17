using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.DataTable;
using GameMain.Builtin;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace GameMain.Hotfix
{
    public static class UIExtension
    {
        /// <summary>
        /// 用于检测是否点击到UI元素
        /// </summary>
        /// <param name="uiComponent">UI组件</param>
        /// <returns>是否点击到UI元素</returns>
#if UNITY_IOS || UNITY_ANDROID
        public static bool IsPointerOverUIObject(this UIComponent uiComponent, Vector3 mousePosition)
        {

            var eventData = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(mousePosition.x, mousePosition.y)
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
#else
        public static bool IsPointerOverUIObject(this UIComponent uiComponent)
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
#endif

        private static IDataTable<DTUITable> GetUITable()
        {
            if (!MainEntry.DataTable.HasDataTable<DTUITable>())
            {
                Log.Error("UI Table is empty.");
                return null;
            }

            return MainEntry.DataTable.GetDataTable<DTUITable>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <param name="uiViews"></param>
        /// <returns></returns>
        private static string GetUIFormAssetName(this UIComponent uiComponent, UIViews uiViews)
        {
            var uiTable = GetUITable();
            if (uiTable != null)
            {
                var uiId = (int)uiViews;
                if (uiTable.HasDataRow(uiId))
                {
                    return AssetUtil.GetUIFormAsset(uiTable.GetDataRow(uiId).AssetName);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 打开UI界面
        /// </summary>
        /// <param name="uiComponent">UI界面组件</param>
        /// <param name="uiViews">UI界面Id</param>
        /// <returns>界面的序列编号</returns>
        public static int OpenUIForm(this UIComponent uiComponent, UIViews uiViews)
        {
            var uiTable = GetUITable();
            if (uiTable != null)
            {
                var uiId = (int)uiViews;
                if (!uiTable.HasDataRow(uiId))
                {
                    Log.Error($"UI Table 不存在id : {uiId}.");
                    return -1;
                }

                var uiRow = uiTable.GetDataRow(uiId);
                var uiAssetName = uiComponent.GetUIFormAssetName(uiViews);
                if (uiComponent.IsLoadingUIForm(uiAssetName))
                {
                    return -1;
                }

                return uiComponent.OpenUIForm(uiAssetName, uiRow.GroupName, uiRow.Priority, uiRow.PauseCovered);
            }

            return -1;
        }

        public static void CloseUIForm(this UIComponent uiComponent, UIViews uiView)
        {
            var uiAssetName = uiComponent.GetUIFormAssetName(uiView);
            if (uiComponent.HasUIForm(uiAssetName))
            {
                var uiForm = uiComponent.GetUIForm(uiAssetName);
                uiComponent.CloseUIForm(uiForm);
            }
        }
    }
}