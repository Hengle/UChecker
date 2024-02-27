using System.Collections.Generic;
using UnityEngine;

namespace UChecker.Editor
{
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

    public struct ReportItem
    {
        public string Info;
        public Object Asset;
    }
}