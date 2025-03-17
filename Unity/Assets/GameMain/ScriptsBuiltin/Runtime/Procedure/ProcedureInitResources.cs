// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/6/24 14:22:24
//  * Description:
//  * Modify Record:
//  *************************************************************/

using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Builtin
{
    public class ProcedureInitResources : ProcedureBase
    {
        private bool mInitResourcesComplete = false;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            mInitResourcesComplete = false;

            MainEntry.Resource.InitResources(OnInitResourcesComplete);
        }


        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!mInitResourcesComplete)
            {
                return;
            }

            ChangeState<ProcedurePreloadHotfix>(procedureOwner);
        }

        private void OnInitResourcesComplete()
        {
            mInitResourcesComplete = true;
            Log.Info("Init Resources complete...");
        }
    }
}