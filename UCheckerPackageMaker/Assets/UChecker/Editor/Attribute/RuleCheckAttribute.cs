using System;
using UnityEngine.Serialization;

namespace UChecker.Editor
{
    [Serializable]
    public class RuleCheckAttribute : System.Attribute
    {
        public string title;
        public string rule;
        public bool enableCheck;
        public bool enableCustomConfig;
        public bool enableFix;
        public int priority;

        public RuleCheckAttribute(string title, string rule, bool enableCheck = true, bool enableCustomConfig = false, bool enableFix = false)
        {
            this.title = title;
            this.rule = rule;
            this.enableCheck = enableCheck;
            this.enableCustomConfig = enableCustomConfig;
            this.enableFix = enableFix;
            this.priority = 0;
        }
        
        public RuleCheckAttribute(string title, string rule,int  priority = 1, bool enableCheck = true, bool enableCustomConfig = false, bool enableFix = false)
        {
            this.title = title;
            this.rule = rule;
            this.enableCheck = enableCheck;
            this.enableCustomConfig = enableCustomConfig;
            this.enableFix = enableFix;
            this.priority = priority;
        }
    }
}