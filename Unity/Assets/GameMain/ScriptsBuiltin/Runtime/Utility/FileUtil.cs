// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/24 14:10:23
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System.IO;
using UnityEngine;

namespace GameMain.Builtin
{
    public static class FileUtil
    {
        public static bool CreateFile(string filePath, bool isCreateDir = true)
        {
            if (!File.Exists(filePath))
            {
                var directory = Path.GetDirectoryName(filePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    if (isCreateDir)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    else
                    {
                        Debug.LogError($"Directory '{directory}' doesn't exist.");
                        return false;
                    }
                }

                File.Create(filePath);
            }

            return true;
        }

        public static bool CreateFile(string filePath, string info, bool isCreateDir = true)
        {
            StreamWriter streamWriter;
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                if (!File.Exists(filePath))
                {
                    var directory = Path.GetDirectoryName(filePath);
                    if (directory != null && !Directory.Exists(directory))
                    {
                        if (isCreateDir)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        else
                        {
                            Debug.LogError($"Directory '{directory}' doesn't exist.");
                            return false;
                        }
                    }
                }

                streamWriter = fileInfo.CreateText();
            }
            else
            {
                streamWriter = fileInfo.AppendText();
            }

            streamWriter.WriteLine(info);
            streamWriter.Close();
            streamWriter.Dispose();

            return true;
        }
    }
}