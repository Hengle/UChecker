using System;
using System.Reflection;
using UnityEngine;

namespace UChecker.Editor
{
    public class FixContext
    {
        private object m_fixObj;
        private MethodInfo m_fixMethod;
        private object[] m_fixMethodParams;
        public const string FIX_FUNCTION_NAME = "Fix";
        public const int FIX_FUNCTION_PARAM_COUNT = 4;
        public void ReadFixType(string fixType)
        {
            Debug.Log(fixType);
            if (string.IsNullOrEmpty(fixType))
            {
                m_fixObj = null;
                m_fixMethod = null;
                m_fixMethodParams = null;
                return;
            }

            Type type = Type.GetType(fixType);
            if (type != null)
            {
                m_fixObj = System.Activator.CreateInstance(type);
                m_fixMethod = type.GetMethod(FIX_FUNCTION_NAME);
                m_fixMethodParams = new object[FIX_FUNCTION_PARAM_COUNT];
            }
        }
        public bool TryFix(ICheck rule,string assetPath, ECheckResult checkResult, ConfigCell cell) 
        {
            if (m_fixMethod == null)
            {
                return false;
            }
            m_fixMethodParams[0] = assetPath;
            m_fixMethodParams[1] = checkResult;
            m_fixMethodParams[2] = rule;
            m_fixMethodParams[3] = cell;
            var suc = m_fixMethod?.Invoke(m_fixObj, m_fixMethodParams);
            return suc != null && (bool)suc;
        }

    }

}