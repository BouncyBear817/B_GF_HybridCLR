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
using System.Text;
using GameMain.Builtin;
using GameMain.Hotfix;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public partial class GameConfigGenerator
    {
        private static List<string> mDataTableVarTypes = null;

        [ToolsMenuMethod("Game Config/Refresh All DataTables", null, 2, 3)]
        public static void RefreshAllDataTables()
        {
            var excelFiles = GetAllGameConfigExcelFullPathList(GameConfigType.DataTable);

            RefreshDataTables(excelFiles);

            GenerateGroupEnumScript();

            GenerateUIViewScript();
        }

        public static void RefreshDataTables(List<string> excelFiles)
        {
            var gameConfig = GameConfigSettings.GetInstance();

            for (int i = 0; i < excelFiles.Count; i++)
            {
                var excelPath = excelFiles[i];
                var outputPath = GetGameConfigExcelOutputPath(GameConfigType.DataTable, excelPath);
                EditorUtility.DisplayProgressBar($"Refreshing DataTable Progress : {i + 1} / {excelFiles.Count}",
                    $"{excelPath} -> {outputPath}",
                    (i + 1) / (float)excelFiles.Count);
                try
                {
                    if (ExcelToTxtFile(excelPath, outputPath))
                    {
                        Debug.Log($"Refreshed DataTable success : {excelPath} -> {outputPath}.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Refreshed DataTable failed. exception message: {e.Message}.");
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            GenerateDataTableCode(gameConfig);
        }

        /// <summary>
        /// 创建 DataTable Excel
        /// </summary>
        /// <param name="excelPath">excel路径</param>
        /// <returns>是否成功创建 DataTable Excel</returns>
        public static bool CreateDataTableExcel(string excelPath)
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
                    sheet.SetValue(2, 2, "ID");
                    sheet.SetValue(3, 1, "#");
                    sheet.SetValue(3, 2, "int");
                    sheet.SetValue(4, 1, "#");
                    sheet.SetValue(4, 3, "备注");
                    sheet.SetValue(4, 4, "请添加字段, 字段名首字母大写");
                    if (mDataTableVarTypes == null)
                    {
                        mDataTableVarTypes = ScanVariableTypes();
                    }

                    if (mDataTableVarTypes != null)
                    {
                        var listValidation = sheet.DataValidations.AddListValidation("D3:Z3");
                        listValidation.AllowBlank = false;
                        listValidation.Formula.Values.Clear();

                        foreach (var type in mDataTableVarTypes)
                        {
                            listValidation.Formula.Values.Add(type);
                        }
                    }

                    var i18nValidation = sheet.DataValidations.AddListValidation("D1:Z1");
                    i18nValidation.AllowBlank = true;
                    i18nValidation.Formula.Values.Clear();
                    i18nValidation.Formula.Values.Add("i18n");
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

        /// <summary>
        /// 生成DataTable对应的脚本
        /// </summary>
        /// <param name="gameConfigSettings">游戏配置设置</param>
        public static void GenerateDataTableCode(GameConfigSettings gameConfigSettings)
        {
            var dataTablesCount = gameConfigSettings.DataTables.Length;
            var outputDir = GetGameConfigPrePath(GameConfigType.DataTable);
            var outputExtension = GetGameConfigExcelOutputFileExtension(GameConfigType.DataTable);

            for (int i = 0; i < dataTablesCount; i++)
            {
                var dataTableName = gameConfigSettings.DataTables[i];
                var dataTablePath = PathUtil.GetCombinePath(outputDir, dataTableName + outputExtension);
                EditorUtility.DisplayProgressBar($"Generate Code Progress : {i + 1} / {dataTablesCount}",
                    $"Generate DataTable Code : {dataTablePath}",
                    (i + 1) / (float)dataTablesCount);
                if (!File.Exists(dataTablePath))
                {
                    Debug.LogWarning($"Generate DataTable Code failed. File is not exist : {dataTablePath}.");
                    continue;
                }

                var dataTableProcessor = DataTableGenerator.Create(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError($"Check Raw Data Failed. DataTableName : {dataTableName}.");
                    continue;
                }

                // DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成组枚举类型
        /// </summary>
        public static void GenerateGroupEnumScript()
        {
            var gamePathSettings = GamePathSettings.GetInstance();
            var excelDir = gamePathSettings.DataTableExcelPath;
            if (!Directory.Exists(excelDir))
            {
                Debug.LogWarning($"Data Table Excel Directory doesn't exist : {excelDir}.");
                return;
            }

            var groupExcelPaths = new string[]
            {
                gamePathSettings.EntityGroupDataTableExcelPath, gamePathSettings.SoundGroupDataTableExcelPath, gamePathSettings.UIGroupDataTableExcelPath
            };

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("/*This code is automatically generated by the tool, please do not manually modify it.*/");
            stringBuilder.AppendLine("namespace GameMain.Hotfix")
                .AppendLine("{")
                .AppendLine("\tpublic static partial class Constant")
                .AppendLine("\t{");
            foreach (var excelPath in groupExcelPaths)
            {
                var excelFilePath = PathUtil.GetCombinePath(excelDir, excelPath);
                if (!File.Exists(excelFilePath))
                {
                    Debug.LogError($"Excel file '{excelFilePath}' doesn't exist.");
                    return;
                }

                var excelPackage = new ExcelPackage(excelFilePath);
                var excelSheet = excelPackage.Workbook.Worksheets["Sheet1"];
                var enumDic = new Dictionary<string, string>();
                for (var rowIndex = excelSheet.Dimension.Start.Row; rowIndex <= excelSheet.Dimension.End.Row; rowIndex++)
                {
                    var rowString = excelSheet.GetValue(rowIndex, 1);
                    if (rowString != null && rowString.ToString().StartsWith("#"))
                    {
                        continue;
                    }

                    var enumName = excelSheet.GetValue(rowIndex, 4).ToString();
                    var commentValue = excelSheet.GetValue(rowIndex, 3);
                    var comment = commentValue == null ? null : commentValue.ToString();
                    enumDic.TryAdd(enumName, comment);
                }

                excelSheet.Dispose();
                excelPackage.Dispose();

                var className = Path.GetFileNameWithoutExtension(excelFilePath);
                var endWithStr = "Table";
                if (className.EndsWith(endWithStr))
                {
                    className = className.Substring(0, className.Length - endWithStr.Length);
                    className = $"E{className}Name";
                }

                stringBuilder.AppendLine($"\t\tpublic enum {className}");
                stringBuilder.AppendLine("\t\t{");
                foreach (var (name, comment) in enumDic)
                {
                    if (!string.IsNullOrEmpty(comment))
                    {
                        stringBuilder
                            .AppendLine("\t\t\t/// <summary>")
                            .AppendLine($"\t\t\t/// {comment}")
                            .AppendLine("\t\t\t/// </summary>");
                    }

                    stringBuilder.AppendLine($"\t\t\t{name},");
                }

                stringBuilder.AppendLine("\t\t}");
            }

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");

            var outFilePath = GamePathSettings.GetInstance().DataTableGroupCodePath;
            try
            {
                File.WriteAllText(outFilePath, stringBuilder.ToString(), Encoding.UTF8);
                Debug.Log($"Generate Group Enum Script Success : {outFilePath}.");
            }
            catch (Exception e)
            {
                Debug.Log($"Generate Group Enum Script Failure : {outFilePath}, Exception : {e}.");
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成UI界面枚举类型
        /// </summary>
        public static void GenerateUIViewScript()
        {
            var excelDir = GamePathSettings.GetInstance().DataTableExcelPath;
            if (!Directory.Exists(excelDir))
            {
                Debug.LogError($"Generate UIView failed! Directory '{excelDir}' is not exist.");
                return;
            }

            var excelFileName = PathUtil.GetCombinePath(excelDir, Constant.UITableExcel);
            if (!File.Exists(excelFileName))
            {
                Debug.LogError($"File '{excelFileName}' is not exist.");
                return;
            }

            var excelPackage = new ExcelPackage(excelFileName);
            var excelSheet = excelPackage.Workbook.Worksheets[0];
            var uiViewDic = new Dictionary<int, string>();
            for (int rowIndex = excelSheet.Dimension.Start.Row; rowIndex <= excelSheet.Dimension.End.Row; rowIndex++)
            {
                var rowStr = excelSheet.GetValue(rowIndex, 1);
                if (rowStr != null && rowStr.ToString().StartsWith("#"))
                {
                    continue;
                }

                uiViewDic.Add(int.Parse(excelSheet.GetValue(rowIndex, 2).ToString()), excelSheet.GetValue(rowIndex, 4).ToString());
            }

            excelSheet.Dispose();
            excelPackage.Dispose();

            var builder = new StringBuilder();
            builder.AppendLine("/*This code is automatically generated by the tool, please do not manually modify it!*/");
            builder.AppendLine();
            builder.AppendLine("namespace GameMain.Hotfix");
            builder.AppendLine("{");
            builder.AppendLine("\tpublic enum UIViews : int");
            builder.AppendLine("\t{");
            foreach (var (key, value) in uiViewDic)
            {
                builder.AppendLine($"\t\t{value} = {key},");
            }

            builder.AppendLine("\t}");
            builder.AppendLine("}");
            File.WriteAllText(Constant.UIViewScriptFile, builder.ToString());
            Debug.Log("Generate UIView.cs success.");
        }
    }
}