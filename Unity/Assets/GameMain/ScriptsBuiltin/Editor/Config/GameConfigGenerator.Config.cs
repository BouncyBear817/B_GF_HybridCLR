// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/09 14:10:01
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public partial class GameConfigGenerator
    {
        [ToolsMenuMethod("Game Config/Refresh All Configs", null, 2, 2)]
        public static void RefreshAllConfigs()
        {
            var excelFiles = GetAllGameConfigExcelFullPathList(GameConfigType.Config);
            RefreshConfigs(excelFiles);
        }

        public static void RefreshConfigs(List<string> excelFiles)
        {
            for (int i = 0; i < excelFiles.Count; i++)
            {
                var excelPath = excelFiles[i];
                var outputPath = GetGameConfigExcelOutputPath(GameConfigType.Config, excelPath);
                EditorUtility.DisplayProgressBar($"Refreshing Config Progress : {i + 1} / {excelFiles.Count}",
                    $"{excelPath} -> {outputPath}",
                    (i + 1) / (float)excelFiles.Count);
                try
                {
                    if (ExcelToTxtFile(excelPath, outputPath))
                    {
                        Debug.Log($"Refreshed Config success : {excelPath} -> {outputPath}.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Refreshed Config failed. exception message: {e.Message}.");
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static bool CreateConfigExcel(string excelPath)
        {
            if (File.Exists(excelPath))
            {
                Debug.LogWarning($"Create Config excel failed. Excel file already exist : {excelPath}.");
                return false;
            }

            try
            {
                var excelDirectory = Path.GetDirectoryName(excelPath);
                if (excelDirectory != null && !Directory.Exists(excelDirectory))
                {
                    Directory.CreateDirectory(excelDirectory);
                }

                using (var excel = new ExcelPackage(excelPath))
                {
                    var sheet = excel.Workbook.Worksheets.Add("Sheet1");
                    sheet.SetValue(1, 1, "#");
                    sheet.SetValue(1, 2, Path.GetFileNameWithoutExtension(excelPath));
                    sheet.SetValue(2, 1, "#");
                    sheet.SetValue(2, 2, "Key");
                    sheet.SetValue(2, 3, "备注");
                    sheet.SetValue(2, 4, "Value");
                    excel.Save();
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Create Config excel failed. Path : {excelPath}, Exception : {e}.");
                return false;
            }
        }
    }
}