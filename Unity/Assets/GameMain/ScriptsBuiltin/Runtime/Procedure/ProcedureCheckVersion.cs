// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/6/24 14:22:49
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Builtin
{
    public class ProcedureCheckVersion : ProcedureBase
    {
        private readonly Dictionary<string, int> mDownloadData = new Dictionary<string, int>();
        private long mUpdateTotalCompressedLength = 0;
        private bool mCheckVersionComplete = false;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Fatal("The device is not connected to the network. Please Check it.");
                return;
            }

            mCheckVersionComplete = false;

            MainEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            MainEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceUpdateAllCompleteEventArgs.EventId, OnResourceUpdateAllComplete);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceVerifyStartEventArgs.EventId, OnResourceVerifyStart);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceVerifySuccessEventArgs.EventId, OnResourceVerifySuccess);
            MainEntry.Event.Subscribe(UnityGameFramework.Runtime.ResourceVerifyFailureEventArgs.EventId, OnResourceVerifyFailure);

            WebRequestVersionList();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mCheckVersionComplete)
            {
                ChangeState<ProcedurePreloadHotfix>(ProcedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            mDownloadData.Clear();
            mUpdateTotalCompressedLength = 0;

            MainEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            MainEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceUpdateAllCompleteEventArgs.EventId, OnResourceUpdateAllComplete);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceVerifyStartEventArgs.EventId, OnResourceVerifyStart);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceVerifySuccessEventArgs.EventId, OnResourceVerifySuccess);
            MainEntry.Event.Unsubscribe(UnityGameFramework.Runtime.ResourceVerifyFailureEventArgs.EventId, OnResourceVerifyFailure);
        }


        private string GetPlatformName()
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "IOS";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_64
            return "Windows64";
#else
            return "Windows";
#endif
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return "MacOS";
#elif UNITY_STANDALONE_LINUX
            return "Linux";
#elif UNITY_WEBGL
            return "WebGL";
#else
            throw new NotSupportedException($"This platform '{Application.platform}' is not supported.");
#endif
        }

        private void WebRequestVersionList()
        {
            if (MainEntry.Resource.ResourceMode == ResourceMode.Updatable || MainEntry.Resource.ResourceMode == ResourceMode.UpdatableWhilePlaying)
            {
                var versionListPath = GameBuildSettings.GetInstance().GetVersionListPath(GetPlatformName());
                Log.Info(versionListPath);
                //向服务器请求版本信息
                MainEntry.WebRequest.AddWebRequest(versionListPath);
            }
            else
            {
                MainEntry.Resource.InitResources(() =>
                {
                    mCheckVersionComplete = true;
                });
            }
        }

        private void CheckAppVersion(VersionInfo versionInfo)
        {
            if (versionInfo == null)
            {
                Log.Error("Version info is invalid.");
                return;
            }

            var curAppVersion = System.Version.Parse(GameFramework.Version.GameVersion);
            var latestAppVersion = System.Version.Parse(versionInfo.LatestAppVersion);
            if (latestAppVersion > curAppVersion)
            {
                MainEntry.BuiltinUIForm.ShowDialog(new DialogParams("New Version!", versionInfo.AppUpdateDesc,
                    "Update", o =>
                    {
                        Application.OpenURL(versionInfo.AppUpdateUri);
                        GameEntry.Shutdown(ShutdownType.Quit);
                    }, "Later", o =>
                    {
                        if (versionInfo.ForceUpdateApp)
                        {
                            GameEntry.Shutdown(ShutdownType.Quit);
                        }
                        else
                        {
                            CheckVersionAndUpdate(versionInfo);
                        }
                    }));
                return;
            }

            CheckVersionAndUpdate(versionInfo);
        }

        private void CheckVersionAndUpdate(VersionInfo versionInfo)
        {
            if (versionInfo == null)
            {
                Log.Error("Version info is invalid.");
                return;
            }

            MainEntry.Resource.UpdatePrefixUri = PathUtil.GetCombinePath(versionInfo.UpdatePrefixUri);

            CheckVersionListResult checkVersionListResult;
            if (CheckApplicableResourceVersion(versionInfo.ApplicableGameVersion))
            {
                checkVersionListResult = MainEntry.Resource.CheckVersionList(versionInfo.InternalResourceVersion);
            }
            else
            {
                checkVersionListResult = MainEntry.Resource.CheckVersionList(MainEntry.Resource.InternalResourceVersion);
            }

            if (checkVersionListResult == CheckVersionListResult.NeedUpdate)
            {
                Log.Info($"Resource need update, Resource update uri : {versionInfo.UpdatePrefixUri}");
                MainEntry.Resource.UpdateVersionList(versionInfo.VersionListLength, versionInfo.VersionListHashCode, versionInfo.VersionListCompressedLength,
                    versionInfo.VersionListCompressedHashCode, new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure));
            }
            else
            {
                MainEntry.Resource.VerifyResources(OnResourcesVerifyComplete);
            }
        }

        private bool CheckApplicableResourceVersion(string gameVersion)
        {
            var versionArray = gameVersion.Split('|');
            foreach (var version in versionArray)
            {
                var fixVersion = version.Trim();
                if (string.Compare(GameFramework.Version.GameVersion, fixVersion, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
        {
            MainEntry.Resource.CheckResources(OnCheckResourcesComplete);
        }

        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
        {
            Log.Fatal($"Update version list failed, download uri : {downloadUri}, error message : {errorMessage}");
        }

        private void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalCompressedLength)
        {
            mUpdateTotalCompressedLength = updateTotalCompressedLength;

            if (updateCount <= 0)
            {
                Log.Info("Update resource count is zero, nothing to update.");
                mCheckVersionComplete = true;
            }
            else
            {
                Log.Info($"Update resource count is {updateCount}, total length is {updateTotalLength}, total compressed length is {updateTotalCompressedLength}.");
                MainEntry.Resource.UpdateResources(OnUpdateResourcesComplete);
            }
        }

        private void OnUpdateResourcesComplete(IResourceGroup resourceGroup, bool result)
        {
            if (result)
            {
                Log.Info("Update resources complete, init resource.");
                mCheckVersionComplete = true;
            }
            else
            {
                Log.Error("Update resources complete with error.");
            }
        }

        private void OnResourcesVerifyComplete(bool result)
        {
            MainEntry.Resource.CheckResources(OnCheckResourcesComplete);
        }

        private void RefreshDownloadProgress()
        {
            var currentLength = 0;
            foreach (var (_, length) in mDownloadData)
            {
                currentLength += length;
            }

            var progress = currentLength / (float)mUpdateTotalCompressedLength;
            MainEntry.BuiltinUIForm.ShowProgress($"{progress:P1}", progress);
        }

        private void OnWebRequestSuccess(object sender, BaseEventArgs e)
        {
            var webRequest = e as WebRequestSuccessEventArgs;
            if (webRequest != null)
            {
                var webTxt = Encoding.UTF8.GetString(webRequest.GetWebResponseBytes());
                var versionInfo = Utility.Json.ToObject<VersionInfo>(webTxt);
                CheckAppVersion(versionInfo);
            }
        }

        private void OnWebRequestFailure(object sender, BaseEventArgs e)
        {
            var eventArgs = e as WebRequestFailureEventArgs;
            if (eventArgs != null)
            {
                Log.Error($"Check version list failed, error message : {eventArgs.ErrorMessage}.");
            }
        }

        private void OnResourceUpdateStart(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceUpdateStartEventArgs;
            if (eventArgs != null)
            {
                mDownloadData[eventArgs.Name] = 0;
                RefreshDownloadProgress();
            }
        }

        private void OnResourceUpdateChanged(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceUpdateChangedEventArgs;
            if (eventArgs != null)
            {
                mDownloadData[eventArgs.Name] = eventArgs.CurrentLength;
                RefreshDownloadProgress();
            }
        }

        private void OnResourceUpdateSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceUpdateSuccessEventArgs;
            if (eventArgs != null)
            {
                mDownloadData[eventArgs.Name] = eventArgs.CompressedLength;
                RefreshDownloadProgress();
                Log.Info($"Resource '{eventArgs.Name}' update success.");
            }
        }

        private void OnResourceUpdateAllComplete(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceUpdateAllCompleteEventArgs;
            if (eventArgs != null)
            {
                MainEntry.BuiltinUIForm.HideProgress();
                Log.Info("Resource update all complete.");
            }
        }

        private void OnResourceUpdateFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceUpdateFailureEventArgs;
            if (eventArgs != null)
            {
                if (eventArgs.RetryCount >= eventArgs.TotalRetryCount)
                {
                    Log.Error(
                        $"Update resource {eventArgs.Name} failed, download uri {eventArgs.DownloadUri}, retry {eventArgs.RetryCount}, error message : {eventArgs.ErrorMessage}.");
                    return;
                }
                else
                {
                    Log.Warning(
                        $"Update resource {eventArgs.Name} failed, download uri {eventArgs.DownloadUri}, retry {eventArgs.RetryCount}, error message : {eventArgs.ErrorMessage}.");
                }

                mDownloadData.Remove(eventArgs.Name);
                RefreshDownloadProgress();
            }
        }

        private void OnResourceVerifyStart(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceVerifyStartEventArgs;
            if (eventArgs != null)
            {
                Log.Info($"Start verify resource, resource count '{eventArgs.Count}', resource total length '{eventArgs.TotalLength}'.");
            }
        }

        private void OnResourceVerifySuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceVerifySuccessEventArgs;
            if (eventArgs != null)
            {
                Log.Info($"Verify resource success : '{eventArgs.Name}'.");
            }
        }

        private void OnResourceVerifyFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnityGameFramework.Runtime.ResourceVerifyFailureEventArgs;
            if (eventArgs != null)
            {
                Log.Warning($"Verify resource failed : '{eventArgs.Name}'.");
            }
        }
    }
}