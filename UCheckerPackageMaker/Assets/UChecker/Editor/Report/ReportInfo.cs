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
        public void AddAssetError(Object obj, string info)
        {
            ReportItems.Add(new ReportItem()
            {
                Info = info,
                Asset = obj,
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
        public string Info;
        [NonSerialized]
        public Object Asset;
    }
}