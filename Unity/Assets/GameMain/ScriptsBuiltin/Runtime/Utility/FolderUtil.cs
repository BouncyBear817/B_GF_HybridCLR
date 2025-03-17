// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/24 14:10:47
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.IO;

namespace GameMain.Builtin
{
    public static class FolderUtil
    {
        public static bool ClearFolder(string folderPath)
        {
            var folderDirs = new DirectoryInfo(folderPath);
            if (!folderDirs.Exists)
            {
                return false;
            }

            foreach (var file in folderDirs.GetFiles())
            {
                file.Delete();
            }

            foreach (var directory in folderDirs.GetDirectories())
            {
                directory.Delete(true);
            }

            return true;
        }

        public static bool CopyFilesToRootPath(string sourceRootPath, string destRootPath, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var fileNames = Directory.GetFiles(sourceRootPath, "*", searchOption);
            foreach (var fileName in fileNames)
            {
                var destFileName = Path.Combine(destRootPath, fileName.Substring(sourceRootPath.Length));
                var destFileInfo = new FileInfo(destFileName);
                if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }

                File.Copy(fileName, destFileName, true);
            }

            return true;
        }
    }
}