using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class UCheckerWindow : EditorWindow
    {
        [MenuItem("Tools/YScan/Setting",priority = 999)]
        public static void OpenMapLineWindow()
        {
            UCheckerWindow pipeLineWindow = EditorWindow.GetWindow<UCheckerWindow>();
            pipeLineWindow.Show();
            pipeLineWindow.titleContent = new GUIContent("Master Check");
            pipeLineWindow.position = pipeLineWindow.GetWindowRect();
        }
        
        [MenuItem("Tools/YScan/ForceRunAll",priority = 1)]
        public static void ForceRunAll()
        {
            var setting = UCheckConfig.GetConfig();
            List<ReportInfo> reportInfos = new List<ReportInfo>();
            foreach (var commonCheck in setting.CommonChecks)
            {
                commonCheck.Check(out var reportInfo);
                reportInfos.Add(reportInfo);
            }
            // TODO 生成报告
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(reportInfos,Formatting.Indented));
        }
    
        
        public const float DRAW_LINE_AREA_WIDTH = 240;
        public const float MENU_BTN_HEIGHT = 30;
        private  static readonly Vector2 LeftPivot = new Vector2(0, 0);
        private const float LINE_THICK = 3;
        private const float TREE_VIEW_OFFSET = 10;
        private const float MENU_VIEW_OFFSET = 5;
        private List<TreeViewItem> m_menuTrees = new List<TreeViewItem>();
        private int m_select = 0;
        private GUIStyle btnStyle;
        // Start is called before the first frame update
        private void OnGUI()
        {
            DrawMenuTree();
        }
        private void OnEnable()
        {
            btnStyle = null;
            m_menuTrees = UCheckConfig.GetMenuTreeItems();
            if (m_select>= m_menuTrees.Count)
            {
                m_select = 0;
            }
        }

        private void DrawMenuTree()
        {
            if (btnStyle == null)
            {
                btnStyle = new GUIStyle(GUI.skin.button);
                btnStyle.fontSize = 18;
            }
            // GUIStyle style = GUI.skin.button;
            // GUIStyle labelStyle = GUI.skin.label;
            // style.fontSize = 18;
            // labelStyle.fontSize = 18;
            Color color = GUI.color;
            var area = GetArea();
            GUILayout.BeginArea(area);
            Handles.color = Color.black;
            // TODO 绘制边框分割
            Handles.DrawLine(new Vector2(area.x + DRAW_LINE_AREA_WIDTH, 0), new Vector2(area.x + DRAW_LINE_AREA_WIDTH,100000), LINE_THICK);
            Handles.DrawLine(new Vector2(0,1), new Vector2(area.width, 1), LINE_THICK);
            Handles.color = Color.white;
            GUILayout.BeginVertical();
            GUILayout.Space(1);
            for (int i = 0; i < m_menuTrees.Count; i++)
            {
                var menuTree = m_menuTrees[i];
                var menuIndex = i;
                if (menuIndex == m_select)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = color;
                }
                if (GUILayout.Button(menuTree.Name,btnStyle,GUILayout.Width(DRAW_LINE_AREA_WIDTH-MENU_VIEW_OFFSET), GUILayout.Height(MENU_BTN_HEIGHT)))
                {
                    m_select = menuIndex;
                }
                GUILayout.Space(1);
            }
            GUI.color = color;
            if (m_select<m_menuTrees.Count)
            {
                DrawTreeView(m_menuTrees[m_select].View);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void DrawTreeView(ITreeView menuTree)
        {
            GUILayout.BeginArea(GetTreeViewArea());
            GUI.color = Color.yellow;
            if (GUILayout.Button("保存配置",btnStyle,GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Save", "保存配置？", "Yes", "No"))
                {
                    UCheckConfig.SaveConfig();
                }
            }
            GUI.color = Color.white;
            menuTree.OnGUI(this);
            GUILayout.EndArea();
        }
        
        private Vector2 Size
        {
            get
            {
                Vector2 size = new Vector2(1600, 1080);
                return size;
            }
        }
        
        private Rect GetWindowRect()
        {
            return new Rect(new Vector2(200, 100), Size);
        }
    
        private Rect GetArea()
        {
            Vector2 area = new Vector2(this.Size.x, Size.y);
            return new Rect(LeftPivot, area);
        }
        
        private Rect GetTreeViewArea()
        {
            Vector2 area = new Vector2(this.Size.x, Size.y);
            var left = LeftPivot;
            left.x += DRAW_LINE_AREA_WIDTH + TREE_VIEW_OFFSET;
            left.y += TREE_VIEW_OFFSET;
            return new Rect(left, area);
        }
    }
}
