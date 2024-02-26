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
        public const string BASIC_SETTING = "基本设置";
        public const float DRAW_LINE_AREA_WIDTH = 240;
        public const float MENU_BTN_HEIGHT = 30;
        private  static readonly Vector2 LeftPivot = new Vector2(0, 0);
        private const float LINE_THICK = 2;
        private const float TREE_VIEW_OFFSET = 5;

        public UCheckerWindow()
        {
            MenuTrees.Add(BASIC_SETTING,new CommonNodeView());
        }
        [MenuItem("Tools/OpenCheckWindow")]
        public static void OpenMapLineWindow()
        {
            UCheckerWindow pipeLineWindow = EditorWindow.GetWindow<UCheckerWindow>();
            pipeLineWindow.Show();
            pipeLineWindow.titleContent = new GUIContent("PCG流水线");
            pipeLineWindow.position = pipeLineWindow.GetWindowRect();
        }
        
        private Dictionary<string,ITreeVIew> MenuTrees = new Dictionary<string,ITreeVIew>();
    
        private string m_select = "";
        // Start is called before the first frame update
        private void OnGUI()
        {
            DrawMenuTree();
        }
        private void OnEnable()
        {
            if (!MenuTrees.ContainsKey(m_select))
            {
                m_select = BASIC_SETTING;
            }
        }

        private void DrawMenuTree()
        {   
            Color color = GUI.color;
            var area = GetArea();
            GUILayout.BeginArea(area);
            Handles.color = Color.black;
            Handles.DrawLine(new Vector2(area.x + DRAW_LINE_AREA_WIDTH, 0), new Vector2(area.x + DRAW_LINE_AREA_WIDTH, area.height), LINE_THICK);
            Handles.DrawLine(new Vector2(0,1), new Vector2(area.width, 1), LINE_THICK);
            Handles.color = Color.white;
            GUILayout.BeginVertical();
            GUILayout.Space(1);
            foreach (var menuTree in MenuTrees)
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
                if (GUILayout.Button(menu.ToString(), GUILayout.Width(DRAW_LINE_AREA_WIDTH-1), GUILayout.Height(MENU_BTN_HEIGHT)))
                {
                    m_select = menu;
                }
                GUILayout.Space(1);
            }
    
            GUI.color = color;
            if (MenuTrees.TryGetValue(m_select,out var treeVIew))
            {
                DrawTreeView(treeVIew);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void DrawTreeView(ITreeVIew menuTree)
        {
            GUILayout.BeginArea(GetTreeViewArea());
            menuTree.OnGUI();
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
