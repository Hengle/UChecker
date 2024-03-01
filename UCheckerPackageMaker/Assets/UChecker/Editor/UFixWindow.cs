using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UChecker.Editor
{
    public class UFixWindow : EditorWindow
    {
        private CommonCheck m_check;
        private HashSet<string> m_fixedAssets = new HashSet<string>();
        private List<ReportItem> m_errorAssets = new List<ReportItem>();

        public const string HelpInfo = "页面功能介绍:1.\n2.\n";
        public static void Open(CommonCheck commonCheck)
        {
            UFixWindow pipeLineWindow = EditorWindow.GetWindow<UFixWindow>();
            pipeLineWindow.Init(commonCheck);
            pipeLineWindow.Show();
            pipeLineWindow.titleContent = new GUIContent("错误资源");
        }
        
        public void Init(CommonCheck commonCheck)
        {
            m_check = commonCheck;
            m_fixedAssets.Clear();
            m_errorAssets.Clear();
            foreach (var fixReport in commonCheck.Report.FixReportItems)
            {
                m_fixedAssets.Add(fixReport.AssetPath);
            }
            foreach (var errorReport in commonCheck.Report.ErrorReportItems)
            {
                m_errorAssets.Add(errorReport);
            }
        }
        
        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            if (m_check==null)
            {
                Close();
                return;
            }
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox(m_check.Setting.Rule,MessageType.Info);
            // EditorGUILayout.HelpBox(HelpInfo,MessageType.Info);
            EditorGUILayout.Space(10);

            if (DrawUtil.DrawHeader("查看白名单资源设置","WhiteList_" + m_check.CheckType,false,true))
            {
                for (int i = 0; i < m_check.Setting.WhiteListAssetPath.Count; i++)
                {
                    var path = m_check.Setting.WhiteListAssetPath[i];
                    var obj =  AssetDatabase.LoadAssetAtPath<Object>(path);
                    EditorGUILayout.ObjectField(obj, typeof(Object),false,GUILayout.MinWidth(50));
                    GUILayout.Label(path,"PreToolbar2",GUILayout.MinWidth(300));
                    if (GUILayout.Button("从白名单移除"))
                    {
                        m_check.Setting.CustomWhiteListPath.RemoveAt(i);
                    }
                }
            }
            
            for (int i = 0; i < m_errorAssets.Count; i++)
            {
                var errorReport = m_errorAssets[i];
                string path = m_errorAssets[i].AssetPath;
                EditorGUILayout.BeginHorizontal();
                var obj =  AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUILayout.ObjectField(obj, typeof(Object),false,GUILayout.MinWidth(50));
                GUILayout.Label(path,"PreToolbar2",GUILayout.MinWidth(300));
                if (GUILayout.Button("选中资源"))
                {
                    Selection.objects = new Object[] { obj };
                }
                bool fixedSource = m_fixedAssets.Contains(path);
                if (!fixedSource)
                {    
                    if (GUILayout.Button("修复资源"))
                    {
                        m_check.FixContext.ReadFixType(m_check.FixType);
                        if (m_check.FixContext.TryFix(m_check.CheckRule, path,ECheckResult.Error,errorReport.Config))
                        {
                            m_fixedAssets.Add(path);
                        }
                    }
                }
                else
                {
                    GUILayout.Button("已修复 ");
                }
                bool inWhiteList = m_check.Setting.WhiteListAssetPath.Contains(path);
                if (inWhiteList)
                {
                    if (GUILayout.Button("从白名单移除"))
                    {
                        m_check.Setting.WhiteListAssetPath.Remove(path);
                    }
                }
                else
                {
                    if (GUILayout.Button("加入白名单"))
                    {
                        m_check.Setting.WhiteListAssetPath.Add(path);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}