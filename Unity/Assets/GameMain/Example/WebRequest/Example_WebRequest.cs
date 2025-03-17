// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/5/13 17:16:10
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.Collections;
using GameFramework;
using GameFramework.Event;
using GameMain.Builtin;
using UnityEngine;

namespace GameMain.Example
{
    public class Example_WebRequest : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);

            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.WebRequestSuccessEventArgs.EventId, WebRequestSuccessHandler);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.WebRequestFailureEventArgs.EventId, WebRequestFailureHandler);

            MainEntry.WebRequest.AddWebRequest("https://car-web-api.autohome.com.cn/car/series/seires_city");
        }
        
        private void WebRequestSuccessHandler(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.WebRequestSuccessEventArgs;
            if (eventArgs != null)
            {
                Debug.Log(Utility.Converter.GetString(eventArgs.GetWebResponseBytes()));
            }
        }

        private void WebRequestFailureHandler(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.WebRequestFailureEventArgs;
            if (eventArgs != null)
            {
                Debug.LogError(eventArgs.ErrorMessage);
            }
        }
    }
}