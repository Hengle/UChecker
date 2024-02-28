using UnityEditor;

namespace UChecker.Editor
{
    [TreeViewAttribute("自定义检查",998)]
    public class CustomTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            var checks =  UCheckConfig.GetConfig().CustomChecks;
            EditorGUILayout.BeginVertical();
            foreach (var check in checks)
            {
                DrawUtil.DrawSetting(check,window);
            }
            EditorGUILayout.EndVertical();
        }
    }
}