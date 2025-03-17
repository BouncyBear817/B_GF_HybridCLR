using UnityEngine;

namespace GameMain.Builtin.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class ProgressForm : BearUIForm
	{
		protected override void OnInit(object userData)
		{
			base.OnInit(userData);
			GetBindComponents(gameObject);

			#region Auto Generate,Do not modify!
			mSlProgress.onValueChanged.AddListener(SlProgressEvent);
			#endregion
		}

		private void SlProgressEvent(float value)
		{
		}

		public void SetProgress(string message, float value)
		{
			mTMessage.text = message;
			mSlProgress.value = value;
		}

		public void SetBackground(Sprite sprite)
		{
			mIBackground.sprite = sprite;
		}

/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
