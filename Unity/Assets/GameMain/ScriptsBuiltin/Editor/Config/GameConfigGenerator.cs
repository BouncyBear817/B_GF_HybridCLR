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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GameFramework;
using GameMain.Builtin;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    public partial class GameConfigGenerator
    {
        private const int MAX_CHAR_LENGTH = 255;

        [InitializeOnLoadMethod]
        static void InitEPPlusLicense()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [ToolsMenuMethod("Game Config/Refresh All", null, 2,  1)]
        public static void RefreshAllGameConfig()
        {
            RefreshAllConfigs();
            RefreshAllDataTables();
            RefreshAllLocalizations();
        }

        /// <summary>
        /// 获取游戏配置表的完整前置路径
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <returns>游戏配置表的完整前置路径</returns>
        public static string GetGameConfigPrePath(GameConfigType configType)
        {
            switch (configType)
            {
                case GameConfigType.DataTable:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().DataTablePath);
                case GameConfigType.Config:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().ConfigPath);
                case GameConfigType.Localization:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().LocalizationPath);
            }

            return "";
        }

        /// <summary>
        /// 获取游戏配置Excel的完整前置路径
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <returns>游戏配置Excel的完整前置路径</returns>
        public static string GetGameConfigExcelPrePath(GameConfigType configType)
        {
            switch (configType)
            {
                case GameConfigType.DataTable:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().DataTableExcelPath);
                case GameConfigType.Config:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().ConfigExcelPath);
                case GameConfigType.Localization:
                    return PathUtil.GetGameConfigFullPath(GamePathSettings.GetInstance().LocalizationExcelPath);
            }

            return "";
        }

        /// <summary>
        ///  获取某个类型下的指定表的相对文件路径
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <param name="configPath">指定表的完整路径</param>
        /// <returns>某个类型下的指定表的相对文件路径</returns>
        public static string GetGameConfigRelativePath(GameConfigType configType, string configPath)
        {
            return Path.GetRelativePath(GetGameConfigPrePath(configType), configPath);
        }

        /// <summary>
        /// 获取某个类型下的指定表的相对文件名称
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <param name="configPath">指定表的完整路径</param>
        /// <returns>某个类型下的指定表的相对文件名称</returns>
        public static string GetGameConfigExcelRelativeFileName(GameConfigType configType, string configPath)
        {
            var relativePath = Path.GetRelativePath(GetGameConfigExcelPrePath(configType), configPath);
            return Utility.Path.GetRegularPath(Path.Combine(Path.GetDirectoryName(relativePath) ?? string.Empty, Path.GetFileNameWithoutExtension(relativePath)));
        }

        public static string GetGameConfigExcelRelativeFullPath(GameConfigType configType, string relativeExcelPath)
        {
            return PathUtil.GetCombinePath(GetGameConfigExcelPrePath(configType), relativeExcelPath + ".xlsx");
        }

        public static List<string> GetGameConfigExcelRelativeFullPath(GameConfigType configType, string[] relativeExcelPaths)
        {
            var result = new List<string>();
            foreach (var t in relativeExcelPaths)
            {
                result.Add(GetGameConfigExcelRelativeFullPath(configType, t));
            }

            return result;
        }

        /// <summary>
        /// 获取某个类型下的所有配置表路径集合
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <param name="searchPattern">搜索模板</param>
        /// <returns>某个类型下的所有配置表路径集合</returns>
        public static List<string> GetAllGameConfigExcelFullPathList(GameConfigType configType, string searchPattern = "*.xlsx")
        {
            var configPath = GetGameConfigExcelPrePath(configType);
            return GetGameConfigListAtDir(configPath, searchPattern);
        }

        /// <summary>
        /// 获取指定excel文件的相对路径
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <param name="excelFile">文件名称</param>
        /// <returns>指定excel文件的相对路径</returns>
        public static string GetGameConfigExcelRelativePath(GameConfigType configType, string excelFile)
        {
            var relativePath = Path.GetRelativePath(GetGameConfigExcelPrePath(configType), excelFile);
            return PathUtil.GetCombinePath(Path.GetDirectoryName(relativePath) ?? string.Empty, Path.GetFileNameWithoutExtension(relativePath));
        }

        /// <summary>
        /// 获取指定excel对应转换的输出路径
        /// </summary>
        /// <param name="configType">游戏配置类型</param>
        /// <param name="excelFile">文件名称</param>
        /// <returns>指定excel对应转换的输出路径</returns>
        public static string GetGameConfigExcelOutputPath(GameConfigType configType, string excelFile)
        {
            var relativePath = GetGameConfigExcelRelativePath(configType, excelFile);
            var extension = GetGameConfigExcelOutputFileExtension(configType);
            return PathUtil.GetCombinePath(GetGameConfigPrePath(configType), relativePath + extension);
        }

        private static string GetGameConfigExcelOutputFileExtension(GameConfigType configType)
        {
            switch (configType)
            {
                case GameConfigType.DataTable:
                case GameConfigType.Config:
                    return ".txt";
                case GameConfigType.Localization:
                    return ".json";
            }

            return "";
        }

        private static List<string> GetGameConfigListAtDir(string configPath, string searchPattern)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(configPath) || !Directory.Exists(configPath))
            {
                return result;
            }

            return GetFiles(configPath, searchPattern, SearchOption.AllDirectories);
        }

        private static List<string> GetFiles(string configPath, string searchPattern, SearchOption searchOption)
        {
            var files = Directory.GetFiles(configPath, searchPattern, searchOption);
            var result = new List<string>();
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).StartsWith("~$"))
                {
                    continue;
                }

                result.Add(file);
            }

            return result;
        }

        private static List<string> ScanVariableTypes()
        {
            var types = new List<string>();
            var nestedTypes = typeof(DataTableProcessor).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var nestedType in nestedTypes)
            {
                if (nestedType.IsClass && nestedType.BaseType != null && nestedType.BaseType.IsGenericType &&
                    string.Compare(nestedType.BaseType.Name, "GenericDataProcessor`1", StringComparison.Ordinal) == 0)
                {
                    var nestedObject = Activator.CreateInstance(nestedType);
                    var type = nestedObject.GetType();
                    var typeName = type.GetProperty("LanguageKeyword")?.GetValue(nestedObject) as string;
                    types.Add(typeName);
                }
            }

            types.Sort();

            var totalLength = 0;
            var result = new List<string>();
            foreach (var type in types)
            {
                totalLength += type.Length;
                if (totalLength > (MAX_CHAR_LENGTH - result.Count))
                {
                    break;
                }

                result.Add(type);
            }

            return result;
        }

        private static bool ExcelToTxtFile(string excelPath, string txtPath)
        {
            bool result;
            var fileInfo = new FileInfo(excelPath);
            var tmpExcelFilePath = PathUtil.GetCombinePath(fileInfo.Directory?.FullName, $"{fileInfo.Name},tmp");
            try
            {
                File.Copy(excelPath, tmpExcelFilePath, true);
                using (var excelPackage = new ExcelPackage(tmpExcelFilePath))
                {
                    var excelWorksheet = excelPackage.Workbook.Worksheets["Sheet1"];
                    var excelTxt = new StringBuilder();
                    var lineTxt = new StringBuilder();
                    for (var rowIndex = excelWorksheet.Dimension.Start.Row; rowIndex <= excelWorksheet.Dimension.End.Row; rowIndex++)
                    {
                        lineTxt.Clear();
                        for (int columnIndex = excelWorksheet.Dimension.Start.Column; columnIndex <= excelWorksheet.Dimension.End.Column; columnIndex++)
                        {
                            var cellContent = excelWorksheet.GetValue<string>(rowIndex, columnIndex);
                            if (!string.IsNullOrEmpty(cellContent))
                            {
                                cellContent = Regex.Replace(cellContent, "\n", @"\n");
                            }

                            lineTxt.Append(cellContent);
                            if (columnIndex < excelWorksheet.Dimension.End.Column)
                            {
                                lineTxt.Append("\t");
                            }
                        }

                        if (string.IsNullOrEmpty(lineTxt.ToString()))
                        {
                            continue;
                        }

                        excelTxt.Append(lineTxt.ToString());
                        if (rowIndex < excelWorksheet.Dimension.End.Row)
                        {
                            excelTxt.AppendLine();
                        }
                    }

                    var outTxtDir = Path.GetDirectoryName(txtPath);
                    if (!string.IsNullOrEmpty(outTxtDir) && !Directory.Exists(outTxtDir))
                    {
                        Directory.CreateDirectory(outTxtDir);
                    }

                    File.WriteAllText(txtPath, excelTxt.ToString(), Encoding.UTF8);

                    result = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Excel to Txt File failed. Path : {excelPath}, Exception : {e}.");
                result = false;
            }

            if (File.Exists(tmpExcelFilePath))
            {
                File.Delete(tmpExcelFilePath);
            }

            return result;
        }
    }
}