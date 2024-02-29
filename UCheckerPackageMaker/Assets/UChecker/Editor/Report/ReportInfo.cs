using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace UChecker.Editor
{
    [Serializable]
    public class ReportInfo
    {
        public string ReportType;
        public string ReportDes;
        public List<ReportItem> ErrorReportItems = new List<ReportItem>();
        public List<ReportItem> FixReportItems = new List<ReportItem>();
        public void AddAssetError(string assetPath,Object obj,string info,ECheckResult result,ConfigCell configCell)
        {
            ErrorReportItems.Add(new ReportItem()
            {
                LogInfo = info,
                Asset = obj,
                AssetPath = assetPath,
                Result = result,
                Config = configCell
            });
        }
        
        public void AddFixAsset(string assetPath)
        {
            FixReportItems.Add(new ReportItem()
            {
                LogInfo = $"Fix Asset: {assetPath}, 检查修复资源并上传",
                AssetPath = assetPath,
                Result = ECheckResult.Fixed,
            });
        }
        public void Clear()
        {
            ErrorReportItems.Clear();
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
        [NonSerialized]
        public ConfigCell Config;
    }
}