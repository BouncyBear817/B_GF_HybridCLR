using GameMain.Builtin;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameMain.Builtin.UI
{
	public partial class DialogForm
	{
		private Text mTTitle;
		private Text mTMessage;
		private Button mBtnConfirm;
		private Text mTConfirmText;
		private Button mBtnCancel;
		private Text mTCancelText;
		private Button mBtnOther;
		private Text mTOtherText;

		private void GetBindComponents(GameObject go)
		{
			var uiAutoBindTool = go.GetComponent<UIAutoBindTool>();

			mTTitle = uiAutoBindTool.GetBindComponent<Text>(0);
			mTMessage = uiAutoBindTool.GetBindComponent<Text>(1);
			mBtnConfirm = uiAutoBindTool.GetBindComponent<Button>(2);
			mTConfirmText = uiAutoBindTool.GetBindComponent<Text>(3);
			mBtnCancel = uiAutoBindTool.GetBindComponent<Button>(4);
			mTCancelText = uiAutoBindTool.GetBindComponent<Text>(5);
			mBtnOther = uiAutoBindTool.GetBindComponent<Button>(6);
			mTOtherText = uiAutoBindTool.GetBindComponent<Text>(7);
		}
	}
}
