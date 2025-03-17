// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/11 13:10:32
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.IO;
using GameFramework;
using UnityEngine;

namespace GameMain.Builtin
{
    public static class PathUtil
    {
        public static string GetGameConfigFullPath(string assetPath)
        {
            return Utility.Path.GetRegularPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetPath));
        }

        public static string GetCombinePath(params string[] paths)
        {
            return Utility.Path.GetRegularPath(Path.Combine(paths));
        }
    }
}