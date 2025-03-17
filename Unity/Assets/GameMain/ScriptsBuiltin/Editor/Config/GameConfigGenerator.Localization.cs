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
using GameMain.Hotfix;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public partial class GameConfigGenerator
    {
        [ToolsMenuMethod("Game Config/Refresh All Localizations", null, 2, 4)]
        public static void RefreshAllLocalizations()
        {
            var excelFiles = GetAllGameConfigExcelFullPathList(GameConfigType.DataTable);

            RefreshDataTables(excelFiles);
        }

        public static void RefreshLocalizations(List<string> excelFiles)
        {
            var gameConfig = GameConfigSettings.GetInstance();

            for (int i = 0; i < excelFiles.Count; i++)
            {
                var excelPath = excelFiles[i];
                var outputPath = GetGameConfigExcelOutputPath(GameConfigType.Localization, excelPath);
                EditorUtility.DisplayProgressBar($"Refreshing Localization Progress : {i + 1} / {excelFiles.Count}",
                    $"{excelPath} -> {outputPath}",
                    (i + 1) / (float)excelFiles.Count);
                try
                {
                    if (ExcelToTxtFile(excelPath, outputPath))
                    {
                        Debug.Log($"Refreshed Localization success : {excelPath} -> {outputPath}.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Refreshed Localization failed. exception message: {e.Message}.");
                }
            }
        }

        public static bool CreateLocalizationExcel(string excelPath)
        {
            if (File.Exists(excelPath))
            {
                Debug.LogWarning($"Create DataTable excel failed. Excel file already exist : {excelPath}.");
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
                Debug.LogError($"Create DataTable excel failed. Path : {excelPath}, Exception : {e}.");
                return false;
            }
        }
    }
}