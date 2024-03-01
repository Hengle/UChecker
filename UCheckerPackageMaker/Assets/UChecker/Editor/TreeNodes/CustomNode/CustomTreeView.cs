using UnityEditor;

namespace UChecker.Editor
{
    [TreeViewAttribute("自定义检查",1)]
    public class CustomTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            EditorGUILayout.BeginVertical();
            if (window.TryGet(ERuleCategory.Custom,out var commonChecks))
            {
                DrawUtil.DrawCommonChecks(commonChecks,window);
            }
            EditorGUILayout.EndVertical();

        }
    }
}