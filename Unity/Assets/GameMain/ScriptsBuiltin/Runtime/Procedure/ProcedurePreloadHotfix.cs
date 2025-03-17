// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/6/24 14:21:43
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
#if !DISABLE_HYBRIDCLR
using HybridCLR;
#endif
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Builtin
{
    public class ProcedurePreloadHotfix : ProcedureBase
    {
        private const float ProgressSpeed = 10f;

        private int mTotalProgress = 0;
        private int mLoadedProgress = 0;
        private float mSmoothProgress = 0;
        private bool mHotfixListIsLoaded = false;

        private Queue<string> mHotfixDllQueue;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

#if !DISABLE_HYBRIDCLR
            MainEntry.Event.Subscribe(LoadHotfixDllEventArgs.EventId, OnLoadHotfixDllCallback);
#endif

            Log.Info("Preload Hotfix...");

            MainEntry.BuiltinUIForm.ShowProgress("0.0%", .0f);
            mLoadedProgress = 0;
            mSmoothProgress = 0;
            mHotfixListIsLoaded = false;
            mHotfixDllQueue = new Queue<string>();

            PreloadHotfixDlls();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mTotalProgress > 0)
            {
                mSmoothProgress = Mathf.Lerp(mSmoothProgress, mLoadedProgress / (float)mTotalProgress, elapseSeconds * ProgressSpeed);
                MainEntry.BuiltinUIForm.ShowProgress($"{mSmoothProgress:P1}", mSmoothProgress);
            }

            if (mLoadedProgress >= mTotalProgress || mSmoothProgress >= 0.99f)
            {
                if (!mHotfixListIsLoaded)
                {
                    return;
                }

                MainEntry.BuiltinUIForm.HideProgress();

                var entryFunc = Utility.Assembly.GetType("GameMain.Hotfix.HotfixEntry")?.GetMethod("StartHotfixLogic", BindingFlags.Public | BindingFlags.Static);
                if (entryFunc == null)
                {
                    Log.Fatal("Start hotfix failed! Not find 'GameMain.Hotfix.HotfixEntry.StartHotfixLogic', please check it.");
                    return;
                }

#if !DISABLE_HYBRIDCLR
                entryFunc.Invoke(null, new object[] { true });
#else
                    entryFunc.Invoke(null, new object[] { false });
#endif
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

#if !DISABLE_HYBRIDCLR
            MainEntry.Event.Unsubscribe(LoadHotfixDllEventArgs.EventId, OnLoadHotfixDllCallback);
#endif
        }

        /// <summary>
        /// 预加载aot库和hotfix库
        /// </summary>
        private void PreloadHotfixDlls()
        {
            mHotfixListIsLoaded = true;

#if !UNITY_EDITOR && !DISABLE_HYBRIDCLR
            mHotfixListIsLoaded = false;
            LoadAotDlls();
            LoadHotfixDlls();
#endif
        }

#if !DISABLE_HYBRIDCLR
        /// <summary>
        /// 加载补充元数据
        /// </summary>
        private void LoadAotDlls()
        {
            var aotMetaDlls = Resources.LoadAll<TextAsset>($"AotDlls");
            mTotalProgress += aotMetaDlls.Length;

            foreach (var aotMetaDll in aotMetaDlls)
            {
                // 为aot assembly加载原始metadata，一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
                var resultCode = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(aotMetaDll.bytes, HybridCLR.HomologousImageMode.SuperSet);
                Log.Info($"Supplement metadata : {aotMetaDll.name}, ret : {resultCode}.");
                if (resultCode == LoadImageErrorCode.OK)
                {
                    mLoadedProgress++;
                }
            }
        }

        /// <summary>
        /// 加载热更新Dll
        /// </summary>
        private void LoadHotfixDlls()
        {
            Log.Info("Start loading hot update dlls.");
            var hotfixListFile = PathUtil.GetCombinePath("Assets", "GameMain/HotfixDlls/HotfixDllList.txt");
            if (MainEntry.Resource.HasAsset(hotfixListFile) == HasAssetResult.NotExist)
            {
                Log.Fatal($"Hotfix dlls is not existed : {hotfixListFile}");
                return;
            }

            MainEntry.Resource.LoadAsset(hotfixListFile, new LoadAssetCallbacks((name, asset, duration, data) =>
            {
                var textAsset = asset as TextAsset;
                if (textAsset != null)
                {
                    var hotfixDlls = Utility.Json.ToObject<string[]>(textAsset.text);
                    foreach (var hotfixDll in hotfixDlls)
                    {
                        mHotfixDllQueue.Enqueue(hotfixDll);
                    }

                    mTotalProgress += mHotfixDllQueue.Count;

                    var dllAsset = AssetUtil.GetHotfixDllAsset(mHotfixDllQueue.Dequeue());
                    MainEntry.Resource.LoadAsset(dllAsset, typeof(TextAsset), new LoadAssetCallbacks(OnLoadHotfixDllSuccess, OnLoadHotfixDllFailure), this);
                }
            }));
        }

        private void OnLoadHotfixDllCallback(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadHotfixDllEventArgs;
            if (eventArgs != null && eventArgs.UserData == this)
            {
                if (eventArgs.Assembly == null)
                {
                    Log.Error($"Load hotfix dll failed : {eventArgs.DllName}");
                    return;
                }

                mLoadedProgress++;
                if (mHotfixDllQueue.Count > 0)
                {
                    var dllAsset = AssetUtil.GetHotfixDllAsset(mHotfixDllQueue.Dequeue());
                    MainEntry.Resource.LoadAsset(dllAsset, typeof(TextAsset), new LoadAssetCallbacks(OnLoadHotfixDllSuccess, OnLoadHotfixDllFailure), this);
                }
            }
        }

        private void OnLoadHotfixDllSuccess(string assetName, object asset, float duration, object userData)
        {
            var dllAsset = asset as TextAsset;
            Assembly assembly = null;
            if (dllAsset != null)
            {
                try
                {
                    assembly = Assembly.Load(dllAsset.bytes);
                    mHotfixListIsLoaded = true;
                }
                catch (Exception e)
                {
                    Log.Error($"Load assembly failed, asset name : {assetName}, error message : {e.Message}.");
                    throw;
                }
            }

            MainEntry.Event.Fire(this, LoadHotfixDllEventArgs.Create(assetName, assembly, userData));
        }

        private void OnLoadHotfixDllFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error($"Load hotfix dll failed, asset name : {assetName}, error message : {errorMessage}.");
            MainEntry.Event.Fire(this, LoadHotfixDllEventArgs.Create(assetName, null, userData));
        }
#endif
    }
}