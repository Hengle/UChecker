using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public static class DrawUtil
    {
        public static void DrawCommonChecks(List<CommonCheck> checks, UCheckerWindow window)
        {
            // var checks =  UCheckConfig.GetConfig().CommonChecks;
            EditorGUILayout.BeginVertical();
            foreach (var check in checks)
            {
                GUILayout.Space(4);
                DrawUtil.DrawSetting(check, window);
            }

            EditorGUILayout.EndVertical();
        }

        public static Dictionary<string, string> m_catchPaths = new Dictionary<string, string>();

        public static void DrawSetting(CommonCheck setting, UCheckerWindow window)
        {
            Color color = GUI.color;
            GUILayout.BeginHorizontal();
            bool isOpen = DrawHeader(setting.Setting.Title, setting.CheckType, false, true);
            GUILayout.BeginHorizontal();
            setting.Setting.EnableFix = DrawEnableBtn("修复开启", "修复关闭", setting.Setting.EnableFix);
            setting.Setting.EnableCheck = DrawEnableBtn("开启检测", "关闭检测", setting.Setting.EnableCheck);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            if (isOpen)
            {
                EditorGUILayout.LabelField($"[规则 ID]:{setting.CheckType}");
                EditorGUILayout.LabelField($"说明:{setting.Setting.Rule}");
                if (setting.Category == ERuleCategory.Template)
                {
                    if (!m_catchPaths.TryGetValue(setting.CheckType, out var dirtyPath))
                    {
                        dirtyPath = setting.Setting.TemplateAssetPath;
                        m_catchPaths.Add(setting.CheckType, dirtyPath);
                    }

                    EditorGUILayout.Space(3);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("设置模板资源");
                    Object obj = null;
                    if (!string.IsNullOrEmpty(dirtyPath))
                    {
                        obj = AssetDatabase.LoadAssetAtPath<Object>(dirtyPath);
                    }
                    obj = EditorGUILayout.ObjectField(obj, typeof(Object), false, GUILayout.MinWidth(20));
                    if (obj != null)
                    {
                        dirtyPath = AssetDatabase.GetAssetPath(obj).Replace("\\", "/");
                    }
                    else
                    {
                        dirtyPath = null;
                    }
                    m_catchPaths[setting.CheckType] = dirtyPath;
                    bool change = setting.Setting.TemplateAssetPath!=dirtyPath;
                    GUI.color = change ? Color.yellow : Color.gray;
                    if (GUILayout.Button("设为模板", GUILayout.Width(200)))
                    {
                        if (obj != null)
                        {
                            setting.Setting.TemplateAssetPath = dirtyPath;
                            Debug.Log($"模板设置：{setting.Setting.TemplateAssetPath}", obj);
                        }
                    }

                    GUI.color = color;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space(3);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("开启单独文件配置", GUILayout.MinWidth(300));
                setting.Setting.EnableCustomConfig = DrawEnableBtn("是", "否", setting.Setting.EnableCustomConfig);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (setting.Setting.EnableCustomConfig)
                {
                    if (setting.Setting.CustomConfigPath.Count == 0)
                    {
                        setting.Setting.CustomConfigPath = new List<ConfigCell>();
                        setting.Setting.CustomConfigPath.Add(new ConfigCell("Assets"));
                    }

                    EditorGUILayout.LabelField($"目标文件夹添加(已添加{setting.Setting.CustomConfigPath.Count}个 )", GUILayout.MinWidth(300));
                    DrawListPath(setting.Setting.CustomConfigPath);
                    EditorGUILayout.LabelField($"忽略的文件夹(已添加{setting.Setting.CustomWhiteListPath.Count}个 )", GUILayout.MinWidth(300));
                    DrawListPath(setting.Setting.CustomWhiteListPath);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUI.color = Color.yellow;
                if (GUILayout.Button("检测", GUILayout.Width(200)))
                {
                    setting.Check();
                }

                GUILayout.Space(5);
                if (GUILayout.Button("开启资源列表", GUILayout.Width(200)))
                {
                    UFixWindow.Open(setting);
                }

                GUI.color = color;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private static bool DrawEnableBtn(string trueFiled, string falseFiled, bool v)
        {
            Color color = GUI.color;
            string txt = v ? trueFiled : falseFiled;
            GUI.color = v ? Color.green : Color.red;
            if (GUILayout.Button(txt, GUILayout.MinWidth(30), GUILayout.MinWidth(20)))
            {
                v = !v;
            }

            // v = EditorGUILayout.Toggle(txt, v, GUILayout.MinWidth(30), GUILayout.Height(20));
            GUI.color = color;
            return v;
        }

        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);
            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;
            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, new GUIStyle("PreToolbar2") { fontSize = 16 }, GUILayout.MinWidth(900))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, new GUIStyle("dragtab") { fontSize = 16 }, GUILayout.MinWidth(900f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        private static Object s_toAdd = null;

        public static void DrawListPath(List<string> listData)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                string path = listData[i];
                Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
                GUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);
                var newFolder = EditorGUILayout.ObjectField(folder, typeof(Object), false, GUILayout.MinWidth(20));
                if (folder != newFolder && newFolder != null)
                {
                    listData[i] = AssetDatabase.GetAssetPath(newFolder);
                }

                GUI.contentColor = folder == null ? Color.red : Color.green;
                GUILayout.Label(path, "PreToolbar2", GUILayout.MinWidth(500));
                GUI.contentColor = Color.white;
                if (GUILayout.Button("删除 -"))
                {
                    listData.RemoveAt(i);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.Space(30);
            s_toAdd = EditorGUILayout.ObjectField(s_toAdd, typeof(Object), false, GUILayout.MinWidth(20));
            if (GUILayout.Button("添加文件夹 +", GUILayout.Width(90)))
            {
                if (s_toAdd != null)
                {
                    string path = AssetDatabase.GetAssetPath(s_toAdd);
                    listData.Add(path);
                }

                s_toAdd = null;
            }

            EditorGUILayout.LabelField("文件夹拖拽上再点添加");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void DrawListPath(List<ConfigCell> listData)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                var cell = listData[i];
                string path = listData[i].FolderPath;
                Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
                GUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);
                var newFolder = EditorGUILayout.ObjectField(folder, typeof(Object), false, GUILayout.MinWidth(20));
                if (folder != newFolder && newFolder != null)
                {
                    listData[i].FolderPath = AssetDatabase.GetAssetPath(newFolder);
                }

                GUI.contentColor = folder == null ? Color.red : Color.green;
                GUILayout.Label(path, "PreToolbar2", GUILayout.MinWidth(500));
                GUI.contentColor = Color.white;
                if (GUILayout.Button("删除 -"))
                {
                    listData.RemoveAt(i);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                for (int j = 0; j < cell.Params.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space(25);
                    var config = cell.Params[j];
                    GUILayout.Label("字段名:");
                    config.FieldName = GUILayout.TextField(config.FieldName, GUILayout.MinWidth(200));
                    GUILayout.Label("值:");
                    config.Value = GUILayout.TextField(config.Value, GUILayout.MinWidth(200));
                    if (GUILayout.Button("删除 -"))
                    {
                        cell.Params.RemoveAt(j);
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.Space(25);
                if (GUILayout.Button("添加参数 +", GUILayout.Width(80)))
                {
                    cell.Params.Add(new CellParam()
                    {
                        FieldName = "FieldName",
                        Value = "填写值",
                    });
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUILayout.Label("-----------------------------------------------------------------");
            GUILayout.BeginHorizontal();
            EditorGUILayout.Space(30);
            s_toAdd = EditorGUILayout.ObjectField(s_toAdd, typeof(Object), false, GUILayout.MinWidth(20));
            if (GUILayout.Button("添加文件夹 +", GUILayout.Width(90)))
            {
                if (s_toAdd != null)
                {
                    string path = AssetDatabase.GetAssetPath(s_toAdd);
                    listData.Add(new ConfigCell(path));
                }

                s_toAdd = null;
            }

            EditorGUILayout.LabelField("文件夹拖拽上再点添加");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("-----------------------------------------------------------------");
        }
    }
}