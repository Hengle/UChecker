using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class BasicAssetTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            DrawUtil.DrawCommonChecks(UCheckConfig.GetConfig().CommonChecks,window);
        }
    }
}