using System;

namespace UChecker.Editor
{
    [Serializable]
    public class TreeViewAttribute: System.Attribute
    {
        public int priority;
        public string name;
        public TreeViewAttribute(string menuName,int priority = 0)
        {
            name = menuName;
            this.priority = priority;
        }
    }
}