using System;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameMain.Builtin;
using UnityGameFramework.Runtime;

namespace GameMain.Hotfix
{
    /// <summary>
    /// 热更逻辑入口
    /// </summary>
    public class HotfixEntry
    {
        public static async void StartHotfixLogic(bool enableHotfix)
        {
            Log.Info($"Start hotfix logic,  enable : {enableHotfix}");
            AwaitExtension.SubscribeEvent();

            MainEntry.Fsm.DestroyFsm<IProcedureManager>();
            var fsmManager = GameFrameworkEntry.GetModule<IFsmManager>();
            var procedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            var gameConfig = await GameConfigSettings.GetInstanceByTask(nameof(GameConfigSettings));

            var procedures = new GameFramework.Procedure.ProcedureBase[gameConfig.Procedures.Length];
            for (var i = 0; i < gameConfig.Procedures.Length; i++)
            {
                var type = Type.GetType(gameConfig.Procedures[i]);
                if (type != null)
                {
                    procedures[i] = Activator.CreateInstance(type) as GameFramework.Procedure.ProcedureBase;
                }
            }

            procedureManager.Initialize(fsmManager, procedures);
            procedureManager.StartProcedure<ProcedurePreload>();
        }
    }
}