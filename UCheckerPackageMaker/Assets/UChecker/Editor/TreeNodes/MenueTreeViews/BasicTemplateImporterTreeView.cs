using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [TreeViewAttribute("模板资源设置检查",995)]
    public class BasicTemplateImporterTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            EditorGUILayout.HelpBox("TODO",MessageType.Warning);
        }
    }
}