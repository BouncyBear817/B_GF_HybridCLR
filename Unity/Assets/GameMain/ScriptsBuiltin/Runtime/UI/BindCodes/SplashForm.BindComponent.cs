using GameMain.Builtin;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameMain.Builtin.UI
{
	public partial class SplashForm
	{
		private Image mISplash;

		private void GetBindComponents(GameObject go)
		{
			var uiAutoBindTool = go.GetComponent<UIAutoBindTool>();

			mISplash = uiAutoBindTool.GetBindComponent<Image>(0);
		}
	}
}
