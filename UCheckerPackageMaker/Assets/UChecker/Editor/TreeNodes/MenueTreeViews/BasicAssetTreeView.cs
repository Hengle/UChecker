using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [TreeViewAttribute("基本资源检查",998)]
    public class BasicAssetTreeView : ITreeView
    {
        public void OnGUI(UCheckerWindow window)
        {
            if (window.TryGet(ERuleCategory.BasicCheck,out var commonChecks))
            {
                DrawUtil.DrawCommonChecks(commonChecks,window);
            }
        }
    }
}