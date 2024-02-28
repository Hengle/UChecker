using UnityEditor;

namespace UChecker.Editor
{
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