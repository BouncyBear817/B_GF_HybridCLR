using System.Collections;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Example
{
    public class Example_Download : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);

            var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

            downloadComponent.AddDownload(@"C:/Users/hp/Test/1", "https://download.jetbrains.com.cn/rider/JetBrains.Rider-2024.1.4.exe");
            downloadComponent.AddDownload("C:/Users/hp/Test/2", "https://download.jetbrains.com.cn/rider/JetBrains.Rider-2024.1.3.exe");
            downloadComponent.AddDownload("C:/Users/hp/Test/3", "https://download.jetbrains.com.cn/rider/JetBrains.Rider-2024.1.2.exe");
            downloadComponent.AddDownload("C:/Users/hp/Test/4", "https://download.jetbrains.com.cn/rider/JetBrains.Rider-2024.1.1.exe");
        }
    }
}