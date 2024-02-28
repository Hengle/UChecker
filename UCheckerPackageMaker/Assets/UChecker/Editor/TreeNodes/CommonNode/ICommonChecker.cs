using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace UChecker.Editor
{
    [Serializable]
    public class CommonCheck
    {
        public CommonCheckerSetting Setting = new CommonCheckerSetting();
        public string CheckType;
        /// <summary>
        /// 要有个默认 json反序列化问题
        /// </summary>
        public CommonCheck()
        {
            
        }
        public CommonCheck(Type check)
        {
            CheckType = check.FullName;
        }
        public void Check(out ReportInfo reportInfo)
        {
            reportInfo = null;
            Type type = Type.GetType(CheckType);
            if (type!= null)
            {
                var obj = System.Activator.CreateInstance(type);
                if (obj is ICheck)
                {
                    var c = obj as ICheck;
                    c.Check(Setting,out reportInfo);
                }
                else
                {
                    Debug.LogError($"Type is not interface ICheck: {CheckType}");
                }
            }
            else
            {
                Debug.LogError($"No type register {CheckType}");
            }
        }
    }
    public interface ICheck
    {
          void Check(CommonCheckerSetting setting,out ReportInfo reportInfo);
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
        //自定义配置
        public List<ConfigCell> CustomConfigPath = new List<ConfigCell>();
        //白名单
        public List<string> CustomWhiteListPath= new List<string>();
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
    
    public abstract class ConditionBase
    {
        public abstract bool Match(string assetPath);
        public virtual string GetName()
        {
            return "条件名";
        }
    }
}