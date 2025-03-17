// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/6/19 11:20:10
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Builtin
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        protected ProcedureOwner mProcedureOwner;

        public virtual ProcedureOwner ProcedureOwner => mProcedureOwner;

        public virtual bool UseNativeDialog => false;

        public void ChangeStateByType(ProcedureOwner procedureOwner, Type stateType)
        {
            ChangeState(procedureOwner, stateType);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            mProcedureOwner = procedureOwner;
        }

        protected void UnloadAllResources()
        {
            UnloadAllScenes();
            MainEntry.Entity.HideAllLoadedEntities();
            MainEntry.Entity.HideAllLoadingEntities();
            MainEntry.ObjectPool.ReleaseAllUnused();
            MainEntry.Resource.ForceUnloadUnusedAssets(true);
        }

        protected void UnloadAllScenes()
        {
            var loadedSceneAssetNames = MainEntry.Scene.GetLoadedSceneAssetNames();
            var unloadScenes = MainEntry.Scene.GetUnloadingSceneAssetNames();
            foreach (var sceneAssetName in loadedSceneAssetNames)
            {
                var isFind = false;
                foreach (var unloadScene in unloadScenes)
                {
                    isFind = sceneAssetName == unloadScene;
                }

                if (!isFind)
                {
                    MainEntry.Scene.UnloadScene(sceneAssetName);
                }
            }
        }
    }
}