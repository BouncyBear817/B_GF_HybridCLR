// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/18 16:10:23
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework.Event;
using GameMain.Builtin;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Hotfix
{
    public class ProcedureChangeScene : ProcedureBase
    {
        private bool mLoadSceneOver = false;
        private string mNextSceneName = "";

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mLoadSceneOver = false;

            MainEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            MainEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            MainEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);

            MainEntry.Sound.StopAllLoadingSounds();
            MainEntry.Sound.StopAllLoadedSounds();

            MainEntry.Entity.HideAllLoadingEntities();
            MainEntry.Entity.HideAllLoadedEntities();

            foreach (var sceneName in MainEntry.Scene.GetLoadedSceneAssetNames())
            {
                MainEntry.Scene.UnloadScene(sceneName);
            }

            MainEntry.Base.ResetNormalGameSpeed();

            mNextSceneName = procedureOwner.GetData<VarString>("NextScene");
            MainEntry.Scene.LoadScene(AssetUtil.GetSceneAsset(mNextSceneName), this);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!mLoadSceneOver)
            {
                return;
            }

            if (mNextSceneName == "Game")
            {
                ChangeState<ProcedureMenu>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            MainEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            MainEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            MainEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadSceneSuccessEventArgs;
            if (eventArgs != null && eventArgs.UserData == this)
            {
                mLoadSceneOver = true;
                Log.Info($"Load Scene '{mNextSceneName}' Success.");
            }
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadSceneFailureEventArgs;
            if (eventArgs != null && eventArgs.UserData == this)
            {
                Log.Error($"Load scene '{mNextSceneName}' failed, exception '{eventArgs.ErrorMessage}', will restart the framework automatically. ");
                GameEntry.Shutdown(ShutdownType.Restart);
            }
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            var eventArgs = e as LoadSceneUpdateEventArgs;
            if (eventArgs != null && eventArgs.UserData == this)
            {
                Log.Info($"Loading scene progress '{eventArgs.Progress.ToString("P1")}'.");
            }
        }
    }
}