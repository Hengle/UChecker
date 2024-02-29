using System;
using UnityEngine;
using Object = System.Object;

namespace UChecker.Editor
{
    [Serializable]
    public class CommonCheck
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public CommonCheckerSetting Setting = new CommonCheckerSetting();

        /// <summary>
        /// 检查类型
        /// </summary>
        public string CheckType;

        /// <summary>
        /// 修复类型
        /// </summary>
        public string FixType;

        public bool HasFix => !string.IsNullOrEmpty(FixType);

        #region NonSerialized

        [NonSerialized] public ReportInfo Report = new ReportInfo();
        [NonSerialized] public FixContext FixContext = new FixContext();
        [NonSerialized] public ICheck CheckRule;
        #endregion

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

        public void Check()
        {
            FixContext.ReadFixType(FixType);
            Report.ReportType = Setting.Title;
            Report.ReportDes = Setting.Rule;
            Report.ErrorReportItems.Clear();
            Report.FixReportItems.Clear();
            Type type = Type.GetType(CheckType);
            if (type != null)
            {
                var obj = System.Activator.CreateInstance(type);
                if (obj is ICheck)
                {
                   CheckRule = obj as ICheck;
                    var r = CheckRule.CheckAndFix(this);
                    if (r != Report)
                    {
                        Report.ErrorReportItems.AddRange(r.ErrorReportItems);
                        Report.FixReportItems.AddRange(r.FixReportItems);
                    }
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
}