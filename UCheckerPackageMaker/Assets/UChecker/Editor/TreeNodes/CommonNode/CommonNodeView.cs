using UnityEditor;

namespace UChecker.Editor
{
    public class CommonNodeView : ITreeVIew
    {
        public const string VERSION = "1.0.0";
        public void OnGUI()
        {
            EditorGUILayout.LabelField($"Version: {VERSION}");
        }
    }
}