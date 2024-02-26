using System;
using System.Collections.Generic;

namespace UChecker.Editor
{
    public interface ICommonCheck
    {
        CommonCheckerSetting Setting { get; }
        void Check();
        void Fix();
    }
    
    [Serializable]
    public class CommonCheckerSetting
    {
        public string Title;
        public string Rule;
        public bool EnableCheck;
        public bool EnableFix;
        public bool EnableCustomConfig;
        //自定义配置
        public List<string> CustomConfigPath;
        //白名单
        public List<string> CustomWhiteListPath;
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