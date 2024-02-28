using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [TreeViewAttribute("基本资源检查",998)]
    public class BasicAssetTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            DrawUtil.DrawCommonChecks(UCheckConfig.GetConfig().CommonChecks,window);
        }
    }
}