using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UChecker.Editor
{
    public class BaseCommonCheck : ICheck
    {
        protected virtual string[] SearchPattern { get; }

        public virtual void Check(CommonCheckerSetting setting,out ReportInfo reportInfo)
        {
            reportInfo = null;
            if (!setting.EnableCheck)
            {
                return;
            }
            #region 过滤文件夹

            reportInfo = new ReportInfo();

            reportInfo.ReportType = setting.Title;
            reportInfo.ReportDes = setting.Rule;
            
            List<ConfigCell> configs = null;
            List<string>  whiteConfigs  = null;
            if (setting.EnableCustomConfig)
            {
                configs = setting.CustomConfigPath; 
                whiteConfigs = setting.CustomWhiteListPath; 
            }
            else
            {
                var globalConfig = UCheckConfig.GetConfig();
                configs = globalConfig.GlobalDefaultPaths; 
                whiteConfigs = globalConfig.GlobalWhiteListPaths; 
            }
            for (int i = 0; i < configs.Count; i++)
            {
                string folder = configs[i].FolderPath;
                if (!whiteConfigs.Contains(folder))
                {
                    if (Directory.Exists(folder))
                    {
                        ForEachCheckConfigPath(folder,configs[i],reportInfo);
                    }
                    else
                    {
                        Debug.LogError($"{this.GetType().FullName} 配置文件夹不存在 {folder}");
                    }
                }
            }
            #endregion
        }

        protected virtual void ForEachCheckConfigPath(string path,ConfigCell cell,ReportInfo reportInfo)
        {
            for (int i = 0; i < SearchPattern.Length; i++)
            {
                var files = Directory.GetFiles(path, SearchPattern[i], SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string filePath = file.Replace("\\", "/");
                    var t = ForEachCheckAssetPath(filePath,cell,reportInfo,out var asset);
                    if (t == ECheckResult.Error || t == ECheckResult.Warning)
                    {
                        reportInfo.AddAssetError(asset,$"{t.ToString()}: {filePath}");
                    }
                }
            }
        }
        
        protected virtual ECheckResult ForEachCheckAssetPath(string path,ConfigCell cell,ReportInfo reportInfo, out Object asset)
        {
            asset = null;
            return ECheckResult.None;
        }
    }
}