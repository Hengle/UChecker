using System;
using System.Collections.Generic;

namespace UChecker.Editor
{
    public interface ICheck
    {
        ReportInfo CheckAndFix(CommonCheck setting);
    }

    public interface IRuleFixer<T> where T : ICheck
    {
        /// <summary>
        /// 修复资源 成功返回true
        /// </summary>
        /// <returns></returns>
        public bool Fix(string path, ECheckResult result, T check, ConfigCell cell);
    }

    [Serializable]
    public class CommonCheckerSetting
    {
        public string Title;
        public string Rule;
        public bool EnableCheck;
        public bool EnableFix;
        public bool EnableCustomConfig;
        public int Priority;
        public string TemplateAssetPath;
     
        //自定义配置
        public List<ConfigCell> CustomConfigPath = new List<ConfigCell>();
        //白名单
        public List<string> CustomWhiteListPath = new List<string>();
        public List<string> WhiteListAssetPath = new List<string>();
        
        
        [NonSerialized]
        public string TemplateAssetDirtyPath;
    }

    /// <summary>
    /// 配置
    /// </summary>
    [Serializable]
    public class ConfigCell
    {
        public string FolderPath;
        public List<CellParam> Params = new List<CellParam>();

        public ConfigCell(string path)
        {
            FolderPath = path;
        }

        public CellParam TryGetFiled(string fieldName)
        {
            return Params.Find(t => t.FieldName.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<CellParam> TryGetAllFiled(string fieldName)
        {
            return Params.FindAll(t => t.FieldName.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [Serializable]
    public class CellParam
    {
        public string FieldName;
        public string Value;
    }
}