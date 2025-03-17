using System;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Builtin.UI
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class DialogForm : BearUIForm
    {
        private GameFrameworkAction<object> mOnConfirmClick = null;
        private GameFrameworkAction<object> mOnCancelClick = null;
        private GameFrameworkAction<object> mOnOtherClick = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            GetBindComponents(gameObject);

            #region Auto Generate,Do not modify!
			mBtnConfirm.onClick.AddListener(BtnConfirmEvent);
			mBtnCancel.onClick.AddListener(BtnCancelEvent);
			mBtnOther.onClick.AddListener(BtnOtherEvent);
            #endregion
        }

        private void BtnConfirmEvent()
        {
            mOnConfirmClick?.Invoke(this);
            OnClose(false, null);
        }

        private void BtnCancelEvent()
        {
            mOnCancelClick?.Invoke(this);
            OnClose(false, null);
        }

        private void BtnOtherEvent()
        {
            mOnOtherClick?.Invoke(this);
            OnClose(false, null);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            var dialogParams = userData as DialogParams;
            if (dialogParams == null)
            {
                Log.Error("dialogParams is invalid.");
                return;
            }

            InitData();

            RefreshDialog(dialogParams);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            InitData();
        }

        private void InitData()
        {
            mBtnConfirm.gameObject.SetActive(false);
            mBtnCancel.gameObject.SetActive(false);
            mBtnOther.gameObject.SetActive(false);
            
            mTTitle.text = string.Empty;
            mTMessage.text = string.Empty;
            
            mTConfirmText.text = string.Empty;
            mTCancelText.text = string.Empty;
            mTOtherText.text = string.Empty;

            mOnConfirmClick = null;
            mOnCancelClick = null;
            mOnOtherClick = null;
        }

        private void RefreshDialog(DialogParams dialogParams)
        {
            mBtnConfirm.gameObject.SetActive(dialogParams.Mode >= 1);
            mBtnCancel.gameObject.SetActive(dialogParams.Mode >= 2);
            mBtnOther.gameObject.SetActive(dialogParams.Mode >= 3);

            if (dialogParams.IsPauseGame)
            {
                MainEntry.Base.PauseGame();
            }

            mTTitle.text = dialogParams.Title;
            mTMessage.text = dialogParams.Message;
            
            mTConfirmText.text = dialogParams.ConfirmButtonText;
            mTCancelText.text = dialogParams.CancelButtonText;
            mTOtherText.text = dialogParams.OtherButtonText;

            mOnConfirmClick = dialogParams.OnConfirmClick;
            mOnCancelClick = dialogParams.OnCancelClick;
            mOnOtherClick = dialogParams.OnOtherClick;
        }
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
