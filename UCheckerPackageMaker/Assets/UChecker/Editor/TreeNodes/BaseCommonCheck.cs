using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UChecker.Editor
{
    public abstract class BaseCommonCheck : ICheck
    {
        /// <summary>
        /// 过滤文件
        /// </summary>
        protected abstract string[] SearchPattern { get; }

        private object m_fixObj;
        private MethodInfo m_fixMethod;
        private object[] m_fixMethodParams;
        public const int FIX_FUNCTION_PARAM_COUNT = 4;
        public const string FIX_FUNCTION_NAME = "Fix";

        protected CommonCheck m_commonCheck;

        /// <summary>
        /// 检查开始 这里处理白名单 开启检查等逻辑 一般无需重写
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="reportInfo"></param>
        public virtual void CheckAndFix(CommonCheck ctx, out ReportInfo reportInfo)
        {
            m_commonCheck = ctx;
            var setting = ctx.Setting;
            reportInfo = null;
            if (!setting.EnableCheck)
            {
                return;
            }

            if (ctx.Setting.EnableFix)
            {
                InitFix(ctx);
            }

            #region 过滤文件夹

            reportInfo = new ReportInfo();

            reportInfo.ReportType = setting.Title;
            reportInfo.ReportDes = setting.Rule;

            List<ConfigCell> configs = null;
            List<string> whiteConfigs = null;
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
                        ForEachCheckConfigPath(folder, configs[i], reportInfo);
                    }
                    else
                    {
                        Debug.LogError($"{this.GetType().FullName} 配置文件夹不存在 {folder}");
                    }
                }
            }

            #endregion
        }


        /// <summary>
        /// 执行筛选好的路径 并自定加入报告内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cell"></param>
        /// <param name="reportInfo"></param>
        protected virtual void ForEachCheckConfigPath(string path, ConfigCell cell, ReportInfo reportInfo)
        {
            for (int i = 0; i < SearchPattern.Length; i++)
            {
                var files = Directory.GetFiles(path, SearchPattern[i], SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string filePath = file.Replace("\\", "/");
                    var t = ForEachCheckAssetPath(filePath, cell, reportInfo, out var asset);
                    if (m_commonCheck.Setting.EnableFix)
                    {
                        if (IsNeedFix(t))
                        {
                            if (TryFix(filePath, t, cell))
                            {
                                Debug.Log($"修复成功 FixRule: {m_commonCheck.FixType},Rule:{m_commonCheck.CheckType}:{filePath}",AssetDatabase.LoadAssetAtPath<Object>(filePath));
                            }
                            else
                            {
                                Debug.Log($"修复失败 FixRule: {m_commonCheck.FixType},Rule:{m_commonCheck.CheckType}:{filePath}",AssetDatabase.LoadAssetAtPath<Object>(filePath));
                            }
                        }
                    }
                    if (t == ECheckResult.Error || t == ECheckResult.Warning)
                    {
                        reportInfo.AddAssetError(filePath, asset, $"{t.ToString()}: {filePath}", t);
                    }
                }
            }
        }

        private bool TryFix(string assetPath, ECheckResult checkResult, ConfigCell cell)
        {
            if (m_fixMethod == null)
            {
                return false;
            }
            m_fixMethodParams[0] = assetPath;
            m_fixMethodParams[1] = checkResult;
            m_fixMethodParams[2] = this;
            m_fixMethodParams[3] = cell;
            var suc = m_fixMethod?.Invoke(m_fixObj, m_fixMethodParams);
            return suc != null && (bool)suc;
        }

        private bool IsNeedFix(ECheckResult result)
        {
            switch (result)
            {
                case ECheckResult.None:
                case ECheckResult.Pass:
                    return false;
                case ECheckResult.Error:
                case ECheckResult.Warning:
                case ECheckResult.CustomAddError:
                case ECheckResult.CustomAddWarning:
                default:
                    return true;
            }
        }

        private void InitFix(CommonCheck ctx)
        {
            if (string.IsNullOrEmpty(ctx.FixType))
            {
                m_fixObj = null;
                m_fixMethod = null;
                m_fixMethodParams = null;
                return;
            }

            Type type = Type.GetType(ctx.FixType);
            if (type != null)
            {
                Debug.LogError(type.FullName);
                Debug.LogError(type.IsGenericTypeDefinition);
                m_fixObj = System.Activator.CreateInstance(type);
                m_fixMethod = type.GetMethod(FIX_FUNCTION_NAME);
                m_fixMethodParams = new object[FIX_FUNCTION_PARAM_COUNT];
            }
        }

        /// <summary>
        /// 遍历每个asset路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cell"></param>
        /// <param name="reportInfo"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        protected abstract ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset);
    }
}