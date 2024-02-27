using System;
using System.Collections.Generic;

namespace UChecker.Editor
{
    [Serializable]
    public class CommonCheck
    {
        public CommonCheckerSetting Setting = new CommonCheckerSetting();
        public ICheck CheckType;
        public CommonCheck(ICheck ctx)
        {
            CheckType = ctx;
        }
        public void Check()
        {
            CheckType?.Check(Setting);
        }
    }

    public interface ICheck
    {
        void Check(CommonCheckerSetting setting);
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

        public CommonCheckerSetting()
        {
            CustomConfigPath = new List<string>(){"Assets","Assets2"};
            CustomWhiteListPath = new List<string>();
        }
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