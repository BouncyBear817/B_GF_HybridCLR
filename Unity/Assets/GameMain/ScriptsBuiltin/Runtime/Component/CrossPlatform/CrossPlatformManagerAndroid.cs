// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/12 14:32:49
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using UnityEngine;

namespace GameMain.Builtin
{
    /// <summary>
    /// 调用android原生
    /// </summary>
    public partial class CrossPlatformManagerAndroid : ICrossPlatformManager
    {
        private AndroidJavaObject mCurrentActivity;

        private AndroidJavaObject CurrentActivity
        {
            get
            {
                if (mCurrentActivity == null)
                {
                    var androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    mCurrentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return mCurrentActivity;
            }
        }

        public void CallCurrentRunnable(string methodName, bool methodIsStatic, Action action)
        {
            NativeTool.CallRunnable(CurrentActivity, methodName, methodIsStatic, action);
        }

        public void CallCurrentMethod(string methodName, object[] methodParams = null)
        {
            NativeTool.CallMethod(CurrentActivity, methodName, methodParams);
        }

        public T CallCurrentMethod<T>(string methodName, object[] methodParams = null)
        {
            return NativeTool.CallMethod<T>(CurrentActivity, methodName, methodParams);
        }

        public void CallCurrentStaticMethod(string methodName, object[] methodParams = null)
        {
            NativeTool.CallStaticMethod(CurrentActivity, methodName, methodParams);
        }

        public T CallCurrentStaticMethod<T>(string methodName, object[] methodParams = null)
        {
            return NativeTool.CallStaticMethod<T>(CurrentActivity, methodName, methodParams);
        }

        public T GetCurrentFiled<T>(string fieldName)
        {
            return NativeTool.GetFiled<T>(CurrentActivity, fieldName);
        }

        public T GetCurrentStaticField<T>(string fieldName)
        {
            return NativeTool.GetStaticField<T>(CurrentActivity, fieldName);
        }

        public void SetCurrentField<T>(string fieldName, T value)
        {
            NativeTool.SetField<T>(CurrentActivity, fieldName, value);
        }

        public void SetCurrentStaticField<T>(string fieldName, T value)
        {
            NativeTool.SetStaticField<T>(CurrentActivity, fieldName, value);
        }

        public void CallRunnable(string className, object[] classParams, string methodName, bool methodIsStatic, Action action)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            NativeTool.CallRunnable(androidJavaObject, methodName, methodIsStatic, action);
        }

        public void CallMethod(string className, object[] classParams, string methodName, object[] methodParams = null)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            NativeTool.CallMethod(androidJavaObject, methodName, methodParams);
        }

        public T CallMethod<T>(string className, object[] classParams, string methodName, object[] methodParams = null)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            return NativeTool.CallMethod<T>(androidJavaObject, methodName, methodParams);
        }

        public void CallStaticMethod(string className, object[] classParams, string methodName, object[] methodParams = null)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            NativeTool.CallStaticMethod(androidJavaObject, methodName, methodParams);
        }

        public T CallStaticMethod<T>(string className, object[] classParams, string methodName, object[] methodParams = null)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            return NativeTool.CallStaticMethod<T>(androidJavaObject, methodName, methodParams);
        }

        public T GetFiled<T>(string className, object[] classParams, string fieldName)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            return NativeTool.GetFiled<T>(androidJavaObject, fieldName);
        }

        public T GetStaticField<T>(string className, object[] classParams, string fieldName)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            return NativeTool.GetStaticField<T>(androidJavaObject, fieldName);
        }

        public void SetField<T>(string className, object[] classParams, string fieldName, T value)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            NativeTool.SetField<T>(androidJavaObject, fieldName, value);
        }

        public void SetStaticField<T>(string className, object[] classParams, string fieldName, T value)
        {
            var androidJavaObject = NativeTool.CreateAndroidJavaObject(className, classParams);
            NativeTool.SetStaticField<T>(androidJavaObject, fieldName, value);
        }

        /// <summary>
        /// 调起相机
        /// </summary>
        public void HandleCamera()
        {
            CurrentActivity.Call("OpenCamera");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CurrentActivity.Dispose();
        }
    }
}