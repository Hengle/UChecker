using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace UChecker.Editor
{
    [Serializable]
    public class ReportInfo
    {
        public string ReportType;
        public string ReportDes;
        public List<ReportItem> ReportItems = new List<ReportItem>();
        public void AddAssetError(string assetPath,Object obj,string info,ECheckResult result)
        {
            ReportItems.Add(new ReportItem()
            {
                LogInfo = info,
                Asset = obj,
                AssetPath = assetPath,
                Result = result,
            });
        }
        public void Clear()
        {
            ReportItems.Clear();
        }
    }
    [Serializable]
    public struct ReportItem
    {
        public string LogInfo;
        public string AssetPath;
        public ECheckResult Result;
        [NonSerialized]
        public Object Asset;
    }
}