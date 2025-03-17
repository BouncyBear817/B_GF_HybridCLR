// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/6/24 14:21:43
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework;
using GameMain.Builtin;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Hotfix
{
    public class ProcedurePreload : ProcedureBase
    {
        private const float ProgressSpeed = 10f;

        private int mTotalProgress = 0;
        private int mLoadedProgress = 0;
        private float mSmoothProgress = 0;
        private bool mPreloadedAllCompleted = false;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            MainEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            MainEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            MainEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            MainEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);

            Log.Info("Preload Game Config And Init Game Config...");

            mLoadedProgress = 0;
            mSmoothProgress = 0;
            mPreloadedAllCompleted = false;
            mTotalProgress = GameConfigSettings.GetInstance().Configs.Length + GameConfigSettings.GetInstance().DataTables.Length;

            PreloadResources();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mTotalProgress <= 0 || mPreloadedAllCompleted)
            {
                return;
            }

            mSmoothProgress = Mathf.Lerp(mSmoothProgress, mLoadedProgress / (float)mTotalProgress, elapseSeconds * ProgressSpeed);
            MainEntry.BuiltinUIForm.ShowProgress($"{mSmoothProgress:P1}", mSmoothProgress);

            if (mLoadedProgress >= mTotalProgress && mSmoothProgress >= 0.99f)
            {
                mPreloadedAllCompleted = true;
                MainEntry.BuiltinUIForm.HideProgress();

                InitGameFrameworkSettings();

                Log.Info($"Preloaded Game Config complete, enter game scene...");
                procedureOwner.SetData<VarString>("NextScene", "Game");
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            MainEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            MainEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            MainEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            MainEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
        }

        private void PreloadResources()
        {
            LoadConfigs();
            LoadDataTables();
            LoadLocalizations();
        }

        private void LoadConfigs()
        {
            foreach (var config in GameConfigSettings.GetInstance().Configs)
            {
                MainEntry.Config.LoadConfig(config, AssetUtil.GetConfigAsset(config, false, true), this);
            }
        }

        private void LoadDataTables()
        {
            foreach (var dataTable in GameConfigSettings.GetInstance().DataTables)
            {
                MainEntry.DataTable.LoadDataTable(dataTable, AssetUtil.GetDataTableAsset(dataTable, false, true), this);
            }
        }

        private void LoadLocalizations()
        {
            foreach (var localization in GameConfigSettings.GetInstance().Localizations)
            {
                Log.Info(localization);
            }
        }

        private void InitGameFrameworkSettings()
        {
            var entityGroupTb = MainEntry.DataTable.GetDataTable<DTEntityGroupTable>();
            foreach (var entityGroup in entityGroupTb.GetAllDataRows())
            {
                if (MainEntry.Entity.HasEntityGroup(entityGroup.Name))
                {
                    var group = MainEntry.Entity.GetEntityGroup(entityGroup.Name);
                    group.InstanceAutoReleaseInterval = entityGroup.InstanceAutoReleaseInterval;
                    group.InstanceCapacity = entityGroup.InstanceCapacity;
                    group.InstanceExpireTime = entityGroup.InstanceExpireTime;
                    group.InstancePriority = entityGroup.InstancePriority;
                    continue;
                }

                MainEntry.Entity.AddEntityGroup(entityGroup.Name, entityGroup.InstanceAutoReleaseInterval, entityGroup.InstanceCapacity, entityGroup.InstanceExpireTime,
                    entityGroup.InstancePriority);
            }

            var soundGroupTb = MainEntry.DataTable.GetDataTable<DTSoundGroupTable>();
            foreach (var soundGroup in soundGroupTb)
            {
                if (MainEntry.Sound.HasSoundGroup(soundGroup.Name))
                {
                    var group = MainEntry.Sound.GetSoundGroup(soundGroup.Name);
                    group.AvoidBeingReplacedBySamePriority = soundGroup.AvoidBeingReplacedBySamePriority;
                    group.Mute = soundGroup.Mute;
                    group.Volume = soundGroup.Volume;
                    continue;
                }

                MainEntry.Sound.AddSoundGroup(soundGroup.Name, soundGroup.AvoidBeingReplacedBySamePriority, soundGroup.Mute, soundGroup.Volume, soundGroup.SoundAgentCount);
            }

            var uiGroupTb = MainEntry.DataTable.GetDataTable<DTUIGroupTable>();
            foreach (var uiGroup in uiGroupTb)
            {
                if (MainEntry.UI.HasUIGroup(uiGroup.Name))
                {
                    var group = MainEntry.UI.GetUIGroup(uiGroup.Name);
                    group.Depth = uiGroup.Depth;
                    continue;
                }

                MainEntry.UI.AddUIGroup(uiGroup.Name, uiGroup.Depth);
            }
        }

        private void OnLoadConfigSuccess(object sender, BaseEventArgs e)
        {
            var eventArgs = e as LoadConfigSuccessEventArgs;
            if (eventArgs != null)
            {
                if (eventArgs.UserData == this)
                {
                    Log.Info($"Load config success : '{eventArgs.ConfigAssetName}'.");
                    mLoadedProgress++;
                }
            }
        }

        private void OnLoadConfigFailure(object sender, BaseEventArgs e)
        {
            var eventArgs = e as LoadConfigFailureEventArgs;
            if (eventArgs != null)
            {
                Log.Error($"Load config failed : '{eventArgs.ConfigAssetName}', error message : {eventArgs.ErrorMessage}");
            }
        }

        private void OnLoadDataTableSuccess(object sender, BaseEventArgs e)
        {
            var eventArgs = e as LoadDataTableSuccessEventArgs;
            if (eventArgs != null)
            {
                if (eventArgs.UserData == this)
                {
                    Log.Info($"Load data table success : '{eventArgs.DataTableAssetName}'.");
                    mLoadedProgress++;
                }
            }
        }

        private void OnLoadDataTableFailure(object sender, BaseEventArgs e)
        {
            var eventArgs = e as LoadDataTableFailureEventArgs;
            if (eventArgs != null)
            {
                Log.Error($"Load data table failed : '{eventArgs.DataTableAssetName}', error message : {eventArgs.ErrorMessage}");
            }
        }
    }
}