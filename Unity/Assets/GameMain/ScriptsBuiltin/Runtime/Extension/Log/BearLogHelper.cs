// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/9/3 16:31:54
//  * Description:
//  * Modify Record:
//  *************************************************************/

using GameFramework;

namespace GameMain.Builtin
{
    public class BearLogHelper : GameFrameworkLog.ILogHelper
    {
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:
                    BearLogger.Debug(message.ToString());
                    break;
                case GameFrameworkLogLevel.Info:
                    BearLogger.Info(message.ToString());
                    break;
                case GameFrameworkLogLevel.Warning:
                    BearLogger.Warning(message.ToString());
                    break;
                case GameFrameworkLogLevel.Error:
                    BearLogger.Error(message.ToString());
                    break;
                case GameFrameworkLogLevel.Fatal:
                    BearLogger.Fatal(message.ToString());
                    break;
                default:
                    throw new GameFrameworkException(message.ToString());
            }
        }
    }
}