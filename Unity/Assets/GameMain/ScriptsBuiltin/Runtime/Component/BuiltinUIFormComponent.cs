// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/22 15:10:55
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework;
using GameMain.Builtin;
using GameMain.Builtin.UI;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin
{
    public class BuiltinUIFormComponent : GameFrameworkComponent
    {
        [SerializeField] private SplashForm mSplashForm;
        [SerializeField] private ProgressForm mProgressForm;
        [SerializeField] private DialogForm mDialogForm;

        public void InitBuiltinForm()
        {
            mSplashForm.Init(null);
            mSplashForm.gameObject.SetActive(false);

            mProgressForm.Init(null);
            mProgressForm.gameObject.SetActive(false);

            mDialogForm.Init(null);
            mDialogForm.gameObject.SetActive(false);
        }

        public void ShowSplash(GameFrameworkAction completeAction)
        {
            mSplashForm.Open(null);
            mSplashForm.StartSplash(() =>
            {
                completeAction?.Invoke();
                mSplashForm.Close(false, null);
            });
        }

        public void ShowProgress(string message, float progress)
        {
            if (!mProgressForm.gameObject.activeSelf)
            {
                mProgressForm.Open(null);
            }

            mProgressForm.SetProgress(message, progress);
        }

        public void HideProgress()
        {
            mProgressForm.Close(false, null);
        }

        public void ShowDialog(DialogParams dialogParams)
        {
            mDialogForm.Open(dialogParams);
        }

        public void HideDialog()
        {
            mDialogForm.Close(false, null);
        }
    }
}