using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class UCheckerWindow : EditorWindow
    {
        public const float DRAW_LINE_AREA_WIDTH = 240;
        public const float MENU_BTN_HEIGHT = 30;
        private  static readonly Vector2 LeftPivot = new Vector2(0, 0);
        private const float LINE_THICK = 2;
        private const float TREE_VIEW_OFFSET = 5;
        private Dictionary<string,ITreeView> m_menuTrees = new Dictionary<string,ITreeView>();

        [MenuItem("Tools/YScan/OpenCheckWindow")]
        public static void OpenMapLineWindow()
        {
            UCheckerWindow pipeLineWindow = EditorWindow.GetWindow<UCheckerWindow>();
            pipeLineWindow.Show();
            pipeLineWindow.titleContent = new GUIContent("Master Check");
            pipeLineWindow.position = pipeLineWindow.GetWindowRect();
        }
        
        [MenuItem("Tools/YScan/ForceRunAll")]
        public static void ForceRunAll()
        {
            var setting = UCheckWindowConfig.Get();
            foreach (var commonCheck in setting.CommonChecks)
            {
                commonCheck.Check();
            }
        }
        
    
        private string m_select = "";
        // Start is called before the first frame update
        private void OnGUI()
        {
            DrawMenuTree();
        }
        private void OnEnable()
        {
            m_menuTrees = UCheckWindowConfig.GetMenuTrees();
            if (!m_menuTrees.ContainsKey(m_select))
            {
                m_select = UCheckWindowConfig.BASIC_SETTING;
            }
        }

        private void DrawMenuTree()
        {   
            Color color = GUI.color;
            var area = GetArea();
            GUILayout.BeginArea(area);
            Handles.color = Color.black;
            var rect = GetWindowRect();
            Handles.DrawLine(new Vector2(area.x + DRAW_LINE_AREA_WIDTH, 0), new Vector2(area.x + DRAW_LINE_AREA_WIDTH,rect.height * 2), LINE_THICK);
            Handles.DrawLine(new Vector2(0,1), new Vector2(area.width, 1), LINE_THICK);
            Handles.color = Color.white;
            GUILayout.BeginVertical();
            GUILayout.Space(1);
            foreach (var menuTree in m_menuTrees)
            {
                var menu =menuTree.Key;
                if (menu == m_select)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = color;
                }
                if (GUILayout.Button(menu, GUILayout.Width(DRAW_LINE_AREA_WIDTH-1), GUILayout.Height(MENU_BTN_HEIGHT)))
                {
                    m_select = menu;
                }
                GUILayout.Space(1);
            }
    
            GUI.color = color;
            if (m_menuTrees.TryGetValue(m_select,out var treeVIew))
            {
                DrawTreeView(treeVIew);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void DrawTreeView(ITreeView menuTree)
        {
            GUILayout.BeginArea(GetTreeViewArea());
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
