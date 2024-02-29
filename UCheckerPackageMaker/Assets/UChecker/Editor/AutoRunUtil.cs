using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public static class AutoRunUtil
    {
        public const string REPORT_PATH = "YScanReport";
        [MenuItem("Tools/YScan/ForceRunAll",priority = 1)]
        public static void ForceRunAll()
        {
            var setting = UCheckConfig.GetConfig();
            Dictionary<string, ReportInfo> reportInfos = new Dictionary<string, ReportInfo>();
            foreach (var commonCheck in setting.CommonChecks)
            {
                commonCheck.Check();
                reportInfos.Add(commonCheck.CheckType,commonCheck.Report);
            }
            foreach (var reportInfo in reportInfos)
            {
                WriteReport(reportInfo.Value);
            }
            // TODO 生成报告
            WriteReport(reportInfos.Values,"总输出报告");
        }

        public static void WriteReport(ReportInfo reportInfo)
        {
            PreparePath(REPORT_PATH);
            string path = Path.Combine(REPORT_PATH, $"{reportInfo.ReportType.Replace("/","-")}.txt");  // 路径
            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs))
                {
                    streamWriter.WriteLine(reportInfo.ReportType.ToString());
                    streamWriter.WriteLine(reportInfo.ReportDes);
                    foreach (var reportItem in reportInfo.ErrorReportItems)
                    {
                        streamWriter.WriteLine($"{reportItem.LogInfo}");
                    }
                    streamWriter.WriteLine();
                    streamWriter.WriteLine();
                }
            }
        }
        
        public static void WriteReport(IEnumerable<ReportInfo> reportInfos,string pathName)
        {
            PreparePath(REPORT_PATH);
            string path = Path.Combine(REPORT_PATH, $"{pathName}.txt");  // 路径
            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs))
                {
                    foreach (var reportInfo in reportInfos)
                    {
                        streamWriter.WriteLine("-----------------------------------------------------------------------");
                        streamWriter.WriteLine(reportInfo.ReportType.ToString());
                        streamWriter.WriteLine(reportInfo.ReportDes);
                        foreach (var reportItem in reportInfo.ErrorReportItems)
                        {
                            streamWriter.WriteLine($"{reportItem.LogInfo}");
                        }
                        streamWriter.WriteLine();
                        streamWriter.WriteLine("-----------------------------------------------------------------------");
                        streamWriter.WriteLine();
                    }
                }
            }
        }
        
        public static bool PreparePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return true;
        }
    }
}