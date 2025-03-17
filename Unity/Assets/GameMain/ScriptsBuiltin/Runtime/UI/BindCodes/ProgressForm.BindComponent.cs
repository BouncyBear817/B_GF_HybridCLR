using GameMain.Builtin;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameMain.Builtin.UI
{
	public partial class ProgressForm
	{
		private Image mIBackground;
		private Text mTMessage;
		private Slider mSlProgress;

		private void GetBindComponents(GameObject go)
		{
			var uiAutoBindTool = go.GetComponent<UIAutoBindTool>();

			mIBackground = uiAutoBindTool.GetBindComponent<Image>(0);
			mTMessage = uiAutoBindTool.GetBindComponent<Text>(1);
			mSlProgress = uiAutoBindTool.GetBindComponent<Slider>(2);
		}
	}
}
