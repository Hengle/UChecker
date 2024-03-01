using System;
using UnityEngine.Serialization;

namespace UChecker.Editor
{
    /// <summary>
    /// 索引
    /// 默认全关闭 防止对于工程大的检查卡工程
    /// </summary>
    [Serializable]
    public class RuleCheckAttribute : System.Attribute
    {
        public string title;
        public string rule;
        public bool enableCheck;
        public bool enableCustomConfig;
        public bool enableFix;
        public int priority;
        public ERuleCategory category;

        public RuleCheckAttribute(string title, string rule,int  priority = 1, bool enableCheck = false, bool enableCustomConfig = false, bool enableFix = false)
        {
            this.category = ERuleCategory.BasicCheck;
            this.title = title;
            this.rule = rule;
            this.enableCheck = enableCheck;
            this.enableCustomConfig = enableCustomConfig;
            this.enableFix = enableFix;
            this.priority = priority;
        }
        
        public RuleCheckAttribute(ERuleCategory category,string title, string rule, bool enableCheck = false, bool enableCustomConfig = false, bool enableFix = false)
        {
            this.category = category;
            this.title = title;
            this.rule = rule;
            this.enableCheck = enableCheck;
            this.enableCustomConfig = enableCustomConfig;
            this.enableFix = enableFix;
            this.priority = 0;
        }
        
        public RuleCheckAttribute(ERuleCategory category,string title, string rule,int  priority = 1, bool enableCheck = false, bool enableCustomConfig = false, bool enableFix = false)
        {
            this.category = category;
            this.title = title;
            this.rule = rule;
            this.enableCheck = enableCheck;
            this.enableCustomConfig = enableCustomConfig;
            this.enableFix = enableFix;
            this.priority = priority;
        }
    }
}