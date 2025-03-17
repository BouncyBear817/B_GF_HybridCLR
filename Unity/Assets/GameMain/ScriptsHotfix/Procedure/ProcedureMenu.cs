// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/18 16:10:17
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityGameFramework.Runtime;
using GameMain.Builtin;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Hotfix
{
    public class ProcedureMenu : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            Log.Info("Enter Menu Procedure.");
            
            MainEntry.UI.OpenUIForm(UIViews.UIInitRootForm);

            MainEntry.Timer.AddRepeatedTimer(2000, 10, () =>
            {
                Log.Info("Complete!!!");
            });
            
            MainEntry.Timer.AddOnceTimer(5000, () =>
            {
                Log.Info("Complete!!!!!!!");
            });
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}