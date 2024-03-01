using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [TreeViewAttribute("基本设置", 999)]
    public class BasicSettingTreeView : ITreeView
    {
        public const string VERSION = "1.0.0";

        public void OnGUI(UCheckerWindow window)
        {
            var checkSetting = UCheckConfig.GetConfig();
            EditorGUILayout.LabelField($"Version: {VERSION}");
            GUILayout.Label($"目标文件夹添加(已添加{checkSetting.GlobalDefaultPaths.Count}个 )", "PreToolbar2", GUILayout.MinWidth(300));
            DrawUtil.DrawListPath(checkSetting.GlobalDefaultPaths);
            GUILayout.Label($"忽略的文件夹(已添加{checkSetting.GlobalWhiteListPaths.Count}个 )", "PreToolbar2", GUILayout.MinWidth(300));
            DrawUtil.DrawListPath(checkSetting.GlobalWhiteListPaths);
            DrawBasicSetting(window);
        }

        private void DrawBasicSetting(UCheckerWindow window)
        {
            GUILayout.Space(5);
            if (DrawUtil.DrawHeader("检测模块设置", "CheckModuleSetting000", true, true))
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                foreach (var checks in window.Checks.Values)
                {
                    for (int i = 0; i < checks.Count; i++)
                    {
                        GUILayout.Space(2);
                        var setting = checks[i];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        setting.Setting.EnableCheck = EditorGUILayout.Toggle(setting.Setting.Title, setting.Setting.EnableCheck, GUILayout.ExpandWidth(true));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(5);
            if (DrawUtil.DrawHeader("修复模块设置", "CheckModuleSetting001", true, true))
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                foreach (var checks in window.Checks.Values)
                {
                    for (int i = 0; i < checks.Count; i++)
                    {
                        var setting = checks[i];
                        if (setting.HasFix)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            setting.Setting.EnableFix = EditorGUILayout.Toggle($"Fix {setting.Setting.Title}", setting.Setting.EnableFix);
                            EditorGUILayout.TextField($"{setting.FixType}", GUILayout.Width(1000));
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}