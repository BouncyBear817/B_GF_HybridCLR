using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Resource;
using GameMain.Builtin;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Hotfix
{
    public static class AwaitExtension
    {
        private static readonly Dictionary<int, UniTaskCompletionSource<UIFormLogic>> sUIFormTask = new Dictionary<int, UniTaskCompletionSource<UIFormLogic>>();
        private static readonly Dictionary<int, UniTaskCompletionSource<EntityLogic>> sEntityTask = new Dictionary<int, UniTaskCompletionSource<EntityLogic>>();
        private static readonly Dictionary<string, UniTaskCompletionSource<bool>> sDataTableTask = new Dictionary<string, UniTaskCompletionSource<bool>>();
        private static readonly Dictionary<string, UniTaskCompletionSource<bool>> sLoadSceneTask = new Dictionary<string, UniTaskCompletionSource<bool>>();
        private static readonly Dictionary<string, UniTaskCompletionSource<bool>> sUnloadSceneTask = new Dictionary<string, UniTaskCompletionSource<bool>>();

        private static readonly HashSet<int> sDownloadSerialIds = new HashSet<int>();
        private static readonly List<DownloadResult> sDelayReleaseDownloadResults = new List<DownloadResult>();

        private static readonly HashSet<int> sWebSerialIds = new HashSet<int>();
        private static readonly List<WebRequestResult> sDelayReleaseWebRequestResults = new List<WebRequestResult>();

#if UNITY_EDITOR
        private static bool sIsSubscribeEvent = false;
#endif

        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            MainEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            MainEntry.Event.Subscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUIFormFailure);

            MainEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            MainEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            MainEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            MainEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            MainEntry.Event.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
            MainEntry.Event.Subscribe(UnloadSceneFailureEventArgs.EventId, OnUnloadSceneFailure);
            MainEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            MainEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            MainEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            MainEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            MainEntry.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            MainEntry.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

#if UNITY_EDITOR
            sIsSubscribeEvent = true;
#endif
        }

#if UNITY_EDITOR
        private static void TipsSubScribeEvent()
        {
            if (!sIsSubscribeEvent)
            {
                throw new GameFrameworkException("Use await/async extensions must to subscribe event!");
            }
        }
#endif

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        /// <param name="resourceComponent">资源组件</param>
        /// <param name="assetName">资源名称</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载任务</returns>
        public static UniTask<T> LoadAssetsAsync<T>(this ResourceComponent resourceComponent, string assetName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            if (string.IsNullOrEmpty(assetName))
            {
                return new UniTask<T>(null);
            }

            var loadAssetTaskCompletionSource = new UniTaskCompletionSource<T>();
            resourceComponent.LoadAsset(assetName, typeof(T), new LoadAssetCallbacks((name, asset, duration, data) =>
                {
                    var source = loadAssetTaskCompletionSource;
                    loadAssetTaskCompletionSource = null;
                    var tAsset = asset as T;
                    if (tAsset != null)
                    {
                        source.TrySetResult(tAsset);
                    }
                    else
                    {
                        Log.Error($"Load asset failure. Load type is {asset.GetType()}, asset type is {typeof(T)}.");
                        source.TrySetException(new GameFrameworkException($"Load asset failure. Load type is {asset.GetType()}, asset type is {typeof(T)}."));
                    }
                }, (name, status, errorMessage, data) =>
                {
                    Log.Error($"Load asset failure, error message : {errorMessage}.");
                    loadAssetTaskCompletionSource.TrySetException(new GameFrameworkException($"Load asset failure, error message : {errorMessage}."));
                }
            ));

            return loadAssetTaskCompletionSource.Task;
        }

        /// <summary>
        /// 加载多个资源（可等待）
        /// </summary>
        /// <param name="resourceComponent">资源组件</param>
        /// <param name="assetNames">资源名称列表</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载任务</returns>
        public static async UniTask<T[]> LoadAssetsAsync<T>(this ResourceComponent resourceComponent, string[] assetNames) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            if (assetNames == null)
            {
                return null;
            }

            var assets = new T[assetNames.Length];
            var tasks = new UniTask<T>[assetNames.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = resourceComponent.LoadAssetsAsync<T>(assetNames[i]);
            }

            await UniTask.WhenAll(tasks);
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = tasks[i].GetAwaiter().GetResult();
            }

            return assets;
        }

        /// <summary>
        /// 打开UI界面
        /// </summary>
        /// <param name="uiComponent">UI界面组件</param>
        /// <param name="uiViews">UI界面Id</param>
        /// <returns>界面的序列编号</returns>
        public static UniTask<UIFormLogic> OpenUIFormAsync(this UIComponent uiComponent, UIViews uiViews)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var serialId = uiComponent.OpenUIForm(uiViews);
            if (serialId < 0)
            {
                return UniTask.FromResult<UIFormLogic>(null);
            }

            var tcs = new UniTaskCompletionSource<UIFormLogic>();
            sUIFormTask.Add(serialId, tcs);
            return tcs.Task;
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as OpenUIFormSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sUIFormTask.TryGetValue(eventArgs.UIForm.SerialId, out var tcs))
                {
                    tcs.TrySetResult(eventArgs.UIForm.Logic);
                    sUIFormTask.Remove(eventArgs.UIForm.SerialId);
                }
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as OpenUIFormFailureEventArgs;
            if (eventArgs != null)
            {
                if (sUIFormTask.TryGetValue(eventArgs.SerialId, out var tcs))
                {
                    Log.Error(eventArgs.ErrorMessage);
                    tcs.TrySetException(new GameFrameworkException(eventArgs.ErrorMessage));
                    sUIFormTask.Remove(eventArgs.SerialId);
                }
            }
        }

        /// <summary>
        /// 显示实体。（可等待）
        /// </summary>
        /// <param name="entityComponent">实体组件</param>
        /// <param name="entityId">实体编号。</param>
        /// <param name="entityLogicType">实体逻辑类型。</param>
        /// <param name="entityAssetName">实体资源名称。</param>
        /// <param name="entityGroupName">实体组名称。</param>
        /// <param name="priority">加载实体资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        public static UniTask<EntityLogic> ShowEntityAsync(this EntityComponent entityComponent, int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, int priority,
            object userData)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var tcs = new UniTaskCompletionSource<EntityLogic>();
            sEntityTask.Add(entityId, tcs);
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            return tcs.Task;
        }

        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as ShowEntitySuccessEventArgs;
            if (eventArgs != null)
            {
                if (sEntityTask.TryGetValue(eventArgs.Entity.Id, out var tcs))
                {
                    tcs.TrySetResult(eventArgs.Entity.Logic);
                    sEntityTask.Remove(eventArgs.Entity.Id);
                }
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as ShowEntityFailureEventArgs;
            if (eventArgs != null)
            {
                if (sEntityTask.TryGetValue(eventArgs.EntityId, out var tcs))
                {
                    Log.Error(eventArgs.ErrorMessage);
                    tcs.TrySetException(new GameFrameworkException(eventArgs.ErrorMessage));
                    sEntityTask.Remove(eventArgs.EntityId);
                }
            }
        }

        /// <summary>
        /// 加载场景。（可等待）
        /// </summary>
        /// <param name="sceneComponent">场景组件</param>
        /// <param name="sceneAssetName">场景资源名称。</param>
        public static async UniTask<bool> LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var tcs = new UniTaskCompletionSource<bool>();
            if (sUnloadSceneTask.TryGetValue(sceneAssetName, out var unloadSceneTcs))
            {
                await unloadSceneTcs.Task;
            }

            sLoadSceneTask.Add(sceneAssetName, tcs);
            try
            {
                sceneComponent.LoadScene(sceneAssetName);
            }
            catch (Exception e)
            {
                Log.Warning(e);
                tcs.TrySetException(e);
                sLoadSceneTask.Remove(sceneAssetName);
            }

            return await tcs.Task;
        }

        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadSceneSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sLoadSceneTask.TryGetValue(eventArgs.SceneAssetName, out var tcs))
                {
                    tcs.TrySetResult(true);
                    sLoadSceneTask.Remove(eventArgs.SceneAssetName);
                }
            }
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadSceneFailureEventArgs;
            if (eventArgs != null)
            {
                if (sLoadSceneTask.TryGetValue(eventArgs.SceneAssetName, out var tcs))
                {
                    Log.Error(eventArgs.ErrorMessage);
                    tcs.TrySetException(new GameFrameworkException(eventArgs.ErrorMessage));
                    sLoadSceneTask.Remove(eventArgs.SceneAssetName);
                }
            }
        }

        /// <summary>
        /// 卸载场景。（可等待）
        /// </summary>
        /// <param name="sceneComponent">场景组件</param>
        /// <param name="sceneAssetName">场景资源名称。</param>
        public static async UniTask<bool> UnloadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var tcs = new UniTaskCompletionSource<bool>();
            if (sLoadSceneTask.TryGetValue(sceneAssetName, out var loadSceneTcs))
            {
                await loadSceneTcs.Task;
            }

            sUnloadSceneTask.Add(sceneAssetName, tcs);
            try
            {
                sceneComponent.UnloadScene(sceneAssetName);
            }
            catch (Exception e)
            {
                Log.Warning(e);
                tcs.TrySetException(e);
                sUnloadSceneTask.Remove(sceneAssetName);
            }

            return await tcs.Task;
        }

        private static void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnloadSceneSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sUnloadSceneTask.TryGetValue(eventArgs.SceneAssetName, out var tcs))
                {
                    tcs.TrySetResult(true);
                    sUnloadSceneTask.Remove(eventArgs.SceneAssetName);
                }
            }
        }

        private static void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as UnloadSceneFailureEventArgs;
            if (eventArgs != null)
            {
                if (sUnloadSceneTask.TryGetValue(eventArgs.SceneAssetName, out var tcs))
                {
                    Log.Error($"Unload scene 'e{eventArgs.SceneAssetName}' failed.");
                    tcs.TrySetException(new GameFrameworkException($"Unload scene 'e{eventArgs.SceneAssetName}' failed."));
                    sUnloadSceneTask.Remove(eventArgs.SceneAssetName);
                }
            }
        }

        /// <summary>
        /// 加载数据表（可等待）
        /// </summary>
        /// <param name="dataTableComponent">数据表组件</param>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="userData">用户自定义数据</param>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>数据表实例 </returns>
        public static async UniTask<IDataTable<T>> LoadDataTableAsync<T>(this DataTableComponent dataTableComponent, string dataTableName, object userData = null) where T : IDataRow
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var dataTable = dataTableComponent.GetDataTable<T>();
            if (dataTable != null)
            {
                return await UniTask.FromResult(dataTable);
            }

            var tcs = new UniTaskCompletionSource<bool>();
            var assetName = AssetUtil.GetDataTableAsset(dataTableName, false, true);
            dataTableComponent.LoadDataTable(dataTableName, assetName, userData);
            sDataTableTask.Add(assetName, tcs);
            var isLoaded = await tcs.Task;
            dataTable = isLoaded ? dataTableComponent.GetDataTable<T>() : null;
            return await UniTask.FromResult(dataTable);
        }

        private static void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadDataTableSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sDataTableTask.TryGetValue(eventArgs.DataTableAssetName, out var tcs))
                {
                    Log.Info($"Load Data Table '{eventArgs.DataTableAssetName}' Success.");
                    tcs.TrySetResult(true);
                    sDataTableTask.Remove(eventArgs.DataTableAssetName);
                }
            }
        }

        private static void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadDataTableFailureEventArgs;
            if (eventArgs != null)
            {
                if (sDataTableTask.TryGetValue(eventArgs.DataTableAssetName, out var tcs))
                {
                    Log.Error($"Load Data Table '{eventArgs.DataTableAssetName}' Failed, error message : {eventArgs.ErrorMessage}.");
                    tcs.TrySetResult(false);
                    sDataTableTask.Remove(eventArgs.DataTableAssetName);
                }
            }
        }

        /// <summary>
        /// 增加 Web 请求任务。（可等待）
        /// </summary>
        /// <param name="webRequestComponent">web请求组件</param>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public static UniTask<WebRequestResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri, WWWForm wwwForm = null, object userData = null)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var tcs = new UniTaskCompletionSource<WebRequestResult>();
            var serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm, AwaitResultWrap<WebRequestResult>.Create(userData, tcs));
            sWebSerialIds.Add(serialId);
            return tcs.Task;
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as WebRequestSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sWebSerialIds.Contains(eventArgs.SerialId))
                {
                    if (eventArgs.UserData is AwaitResultWrap<WebRequestResult> webRequestResultWarp)
                    {
                        var result = WebRequestResult.Create(eventArgs.GetWebResponseBytes(), false, string.Empty, webRequestResultWarp.UserData);
                        webRequestResultWarp.Source.TrySetResult(result);
                        sDelayReleaseWebRequestResults.Add(result);
                        ReferencePool.Release(webRequestResultWarp);
                    }

                    sWebSerialIds.Remove(eventArgs.SerialId);
                    if (sWebSerialIds.Count == 0)
                    {
                        foreach (var requestResult in sDelayReleaseWebRequestResults)
                        {
                            ReferencePool.Release(requestResult);
                        }

                        sDelayReleaseWebRequestResults.Clear();
                    }
                }
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as WebRequestFailureEventArgs;
            if (eventArgs != null)
            {
                if (sWebSerialIds.Contains(eventArgs.SerialId))
                {
                    if (eventArgs.UserData is AwaitResultWrap<WebRequestResult> webRequestResultWarp)
                    {
                        var result = WebRequestResult.Create(null, true, eventArgs.ErrorMessage, webRequestResultWarp.UserData);
                        webRequestResultWarp.Source.TrySetResult(result);
                        sDelayReleaseWebRequestResults.Add(result);
                        ReferencePool.Release(webRequestResultWarp);
                    }

                    sWebSerialIds.Remove(eventArgs.SerialId);
                    if (sWebSerialIds.Count == 0)
                    {
                        foreach (var requestResult in sDelayReleaseWebRequestResults)
                        {
                            ReferencePool.Release(requestResult);
                        }

                        sDelayReleaseWebRequestResults.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 增加下载任务（可等待）
        /// </summary>
        /// <param name="downloadComponent">下载组件</param>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务的序列编号</returns>
        public static UniTask<DownloadResult> AddDownloadAsync(this DownloadComponent downloadComponent, string downloadPath, string downloadUri, object userData = null)
        {
#if UNITY_EDITOR
            TipsSubScribeEvent();
#endif
            var tcs = new UniTaskCompletionSource<DownloadResult>();
            var serialId = downloadComponent.AddDownload(downloadPath, downloadUri, AwaitResultWrap<DownloadResult>.Create(userData, tcs));
            sDownloadSerialIds.Add(serialId);
            return tcs.Task;
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as DownloadSuccessEventArgs;
            if (eventArgs != null)
            {
                if (sDownloadSerialIds.Contains(eventArgs.SerialId))
                {
                    if (eventArgs.UserData is AwaitResultWrap<DownloadResult> resultWarp)
                    {
                        var result = DownloadResult.Create(false, string.Empty, resultWarp.UserData);
                        resultWarp.Source.TrySetResult(result);
                        sDelayReleaseDownloadResults.Add(result);
                        ReferencePool.Release(resultWarp);
                    }

                    sDownloadSerialIds.Remove(eventArgs.SerialId);
                    if (sDownloadSerialIds.Count == 0)
                    {
                        foreach (var result in sDelayReleaseDownloadResults)
                        {
                            ReferencePool.Release(result);
                        }

                        sDelayReleaseDownloadResults.Clear();
                    }
                }
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as DownloadFailureEventArgs;
            if (eventArgs != null)
            {
                if (sDownloadSerialIds.Contains(eventArgs.SerialId))
                {
                    if (eventArgs.UserData is AwaitResultWrap<DownloadResult> resultWarp)
                    {
                        var result = DownloadResult.Create(true, eventArgs.ErrorMessage, resultWarp.UserData);
                        resultWarp.Source.TrySetResult(result);
                        sDelayReleaseDownloadResults.Add(result);
                        ReferencePool.Release(resultWarp);
                    }

                    sDownloadSerialIds.Remove(eventArgs.SerialId);
                    if (sDownloadSerialIds.Count == 0)
                    {
                        foreach (var result in sDelayReleaseDownloadResults)
                        {
                            ReferencePool.Release(result);
                        }

                        sDelayReleaseDownloadResults.Clear();
                    }
                }
            }
        }
    }
}