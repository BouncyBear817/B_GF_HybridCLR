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
    public partial class CrossPlatformManagerAndroid
    {
        private static class NativeTool
        {
            private static bool IsParamAvailable(object[] param)
            {
                return param != null && param.Length > 0;
            }

            public static AndroidJavaObject CreateAndroidJavaObject(string className, object[] classParams = null)
            {
                AndroidJavaObject androidJavaObject = null;
                androidJavaObject = IsParamAvailable(classParams) ? new AndroidJavaObject(className, classParams) : new AndroidJavaObject(className);

                return androidJavaObject;
            }


            public static void CallRunnable(AndroidJavaObject androidJavaObject, string methodName, bool methodIsStatic, Action action)
            {
                if (action != null)
                {
                    if (methodIsStatic)
                    {
                        androidJavaObject.CallStatic(methodName, new AndroidJavaRunnable(() => { action(); }));
                    }
                    else
                    {
                        androidJavaObject.Call(methodName, new AndroidJavaRunnable(() => { action(); }));
                    }
                }

                androidJavaObject.Dispose();
            }

            public static void CallMethod(AndroidJavaObject androidJavaObject, string methodName, object[] methodParams = null)
            {
                if (IsParamAvailable(methodParams))
                {
                    androidJavaObject.Call(methodName, methodParams);
                }
                else
                {
                    androidJavaObject.Call(methodName);
                }

                androidJavaObject.Dispose();
            }

            public static T CallMethod<T>(AndroidJavaObject androidJavaObject, string methodName, object[] methodParams = null)
            {
                var result = IsParamAvailable(methodParams) ? androidJavaObject.Call<T>(methodName, methodParams) : androidJavaObject.Call<T>(methodName);
                androidJavaObject.Dispose();
                return result;
            }

            public static void CallStaticMethod(AndroidJavaObject androidJavaObject, string methodName, object[] methodParams = null)
            {
                if (IsParamAvailable(methodParams))
                {
                    androidJavaObject.CallStatic(methodName, methodParams);
                }
                else
                {
                    androidJavaObject.CallStatic(methodName);
                }

                androidJavaObject.Dispose();
            }

            public static T CallStaticMethod<T>(AndroidJavaObject androidJavaObject, string methodName, object[] methodParams = null)
            {
                var result = IsParamAvailable(methodParams) ? androidJavaObject.CallStatic<T>(methodName, methodParams) : androidJavaObject.CallStatic<T>(methodName);
                androidJavaObject.Dispose();
                return result;
            }

            public static T GetFiled<T>(AndroidJavaObject androidJavaObject, string fieldName)
            {
                return androidJavaObject.Get<T>(fieldName);
            }

            public static T GetStaticField<T>(AndroidJavaObject androidJavaObject, string fieldName)
            {
                return androidJavaObject.GetStatic<T>(fieldName);
            }

            public static void SetField<T>(AndroidJavaObject androidJavaObject, string fieldName, T value)
            {
                androidJavaObject.Set<T>(fieldName, value);
            }

            public static void SetStaticField<T>(AndroidJavaObject androidJavaObject, string fieldName, T value)
            {
                androidJavaObject.SetStatic<T>(fieldName, value);
            }
        }
    }
}