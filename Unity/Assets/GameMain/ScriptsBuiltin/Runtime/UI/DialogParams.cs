// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/22 10:10:29
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework;

namespace GameMain.Builtin
{
    /// <summary>
    /// 对话框显示数据
    /// </summary>
    public class DialogParams
    {
        /// <summary>
        /// 显示button数量，取值1,2,3
        /// </summary>
        public int Mode { get; private set; }

        /// <summary>
        /// 对话框标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 对话框信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 弹出对话框时是否暂停游戏
        /// </summary>
        public bool IsPauseGame { get; set; }

        /// <summary>
        /// 确认按钮文本
        /// </summary>
        public string ConfirmButtonText { get; set; }

        /// <summary>
        /// 确认按钮回调
        /// </summary>
        public GameFrameworkAction<object> OnConfirmClick { get; set; }

        /// <summary>
        /// 取消按钮文本
        /// </summary>
        public string CancelButtonText { get; set; }

        /// <summary>
        /// 取消按钮回调
        /// </summary>
        public GameFrameworkAction<object> OnCancelClick { get; set; }

        /// <summary>
        /// 其它按钮文本
        /// </summary>
        public string OtherButtonText { get; set; }

        /// <summary>
        /// 其它按钮回调
        /// </summary>
        public GameFrameworkAction<object> OnOtherClick { get; set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; set; }

        public DialogParams(string title, string message, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, object userData = null)
        {
            Mode = 1;
            Title = title;
            Message = message;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            UserData = userData;
        }

        public DialogParams(string title, string message, bool isPauseGame, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, object userData = null)
        {
            Mode = 1;
            Title = title;
            Message = message;
            IsPauseGame = isPauseGame;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            UserData = userData;
        }

        public DialogParams(string title, string message, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, string cancelButtonText,
            GameFrameworkAction<object> onCancelClick, object userData = null)
        {
            Mode = 2;
            Title = title;
            Message = message;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            CancelButtonText = cancelButtonText;
            OnCancelClick = onCancelClick;
            UserData = userData;
        }

        public DialogParams(string title, string message, bool isPauseGame, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, string cancelButtonText,
            GameFrameworkAction<object> onCancelClick, object userData = null)
        {
            Mode = 2;
            Title = title;
            Message = message;
            IsPauseGame = isPauseGame;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            CancelButtonText = cancelButtonText;
            OnCancelClick = onCancelClick;
            UserData = userData;
        }

        public DialogParams(string title, string message, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, string cancelButtonText,
            GameFrameworkAction<object> onCancelClick, string otherButtonText, GameFrameworkAction<object> onOtherClick, object userData = null)
        {
            Mode = 3;
            Title = title;
            Message = message;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            CancelButtonText = cancelButtonText;
            OnCancelClick = onCancelClick;
            OtherButtonText = otherButtonText;
            OnOtherClick = onOtherClick;
            UserData = userData;
        }

        public DialogParams(string title, string message, bool isPauseGame, string confirmButtonText, GameFrameworkAction<object> onConfirmClick, string cancelButtonText,
            GameFrameworkAction<object> onCancelClick, string otherButtonText, GameFrameworkAction<object> onOtherClick, object userData = null)
        {
            Mode = 3;
            Title = title;
            Message = message;
            IsPauseGame = isPauseGame;
            ConfirmButtonText = confirmButtonText;
            OnConfirmClick = onConfirmClick;
            CancelButtonText = cancelButtonText;
            OnCancelClick = onCancelClick;
            OtherButtonText = otherButtonText;
            OnOtherClick = onOtherClick;
            UserData = userData;
        }
    }
}