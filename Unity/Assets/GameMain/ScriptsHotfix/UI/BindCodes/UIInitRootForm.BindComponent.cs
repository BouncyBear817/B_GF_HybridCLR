using GameMain.Builtin;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameMain.Hotfix.UI
{
	public partial class UIInitRootForm
	{
		private RectTransform mTransLaunchView;
		private RawImage mRILaunchView;

		private void GetBindComponents(GameObject go)
		{
			var uiAutoBindTool = go.GetComponent<Builtin.UIAutoBindTool>();

			mTransLaunchView = uiAutoBindTool.GetBindComponent<RectTransform>(0);
			mRILaunchView = uiAutoBindTool.GetBindComponent<RawImage>(1);
		}
	}
}
