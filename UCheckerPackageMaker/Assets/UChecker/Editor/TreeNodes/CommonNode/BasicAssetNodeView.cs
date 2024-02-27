﻿using UnityEditor;

namespace UChecker.Editor
{
    public class BasicAssetNodeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            var checks =  UCheckConfig.GetConfig().CommonChecks;
            EditorGUILayout.BeginVertical();
            foreach (var check in checks)
            {
                DrawUtil.DrawSetting(check,window);
            }
            EditorGUILayout.EndVertical();
        }
    }
}