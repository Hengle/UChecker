using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class BasicSettingNodeView : ITreeView
    {
        public const string VERSION = "1.0.0";
        public void OnGUI(UCheckerWindow window)
        {
            var checkSetting = UCheckConfig.GetConfig();
            EditorGUILayout.LabelField($"Version: {VERSION}");
            GUILayout.Label($"目标文件夹添加(已添加{checkSetting.GlobalDefaultPaths.Count}个 )", "PreToolbar2",GUILayout.MinWidth(300));
            DrawUtil.DrawListPath(checkSetting.GlobalDefaultPaths);
            GUILayout.Label($"忽略的文件夹(已添加{checkSetting.GlobalWhiteListPaths.Count}个 )", "PreToolbar2",GUILayout.MinWidth(300));
            DrawUtil.DrawListPath(checkSetting.GlobalWhiteListPaths);
        }
    }
}