using UnityEngine;
using UnityEngine.Events;

namespace GameMain.Builtin.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class SplashForm: BearUIForm
	{
		private UnityAction mCompleteAction;
		private DOTweenSequence mSequence;
		
		protected override void OnInit(object userData)
		{
			base.OnInit(userData);
			GetBindComponents(gameObject);

			#region Auto Generate,Do not modify!
			#endregion
			
			mSequence = mISplash.GetComponent<DOTweenSequence>();
		}

		public void SetBackground(Sprite sprite)
		{
			mISplash.sprite = sprite;
		}

		public void StartSplash(UnityAction completeAction)
		{
			mSequence.DOPlay();
			mCompleteAction = completeAction;
			mSequence.OnComplete.AddListener(mCompleteAction);
		}

		protected override void OnClose(bool isShutdown, object userData)
		{
			base.OnClose(isShutdown, userData);
			mSequence.OnComplete.RemoveListener(mCompleteAction);
		}
		/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
