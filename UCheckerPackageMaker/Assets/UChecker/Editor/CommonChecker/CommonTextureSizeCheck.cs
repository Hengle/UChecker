using System;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class CommonTextureSizeCheck : ICheck
    {
        public void Check(CommonCheckerSetting setting)
        {
            for (int i = 0; i < setting.CustomConfigPath.Count; i++)
            {
                Debug.Log(setting.CustomConfigPath[i]);
            }
        }
    }
}