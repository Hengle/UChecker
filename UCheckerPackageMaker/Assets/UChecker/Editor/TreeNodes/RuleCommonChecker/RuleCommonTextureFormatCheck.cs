using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UChecker.Editor
{
    /// <summary>
    /// 检查问题压缩格式
    /// </summary>
    [RuleCheck("检查移动平台纹理压缩格式","检查纹理压缩格式 默认压缩格式 ASTC6x6 需要其它压缩格式可添加Format多个字段",998)]
    public class RuleCommonTextureFormatCheck : BaseCommonCheck
    {
        public const string FORMAT_FIELD_NAME = "format";
        private List<TextureImporterFormat> m_defaultFormats = new List<TextureImporterFormat>();
        protected override string[] SearchPattern => new[] { "*.png", "*.jpg", "*.tga" };
        public string GetFormatsString()
        {
            StringBuilder formats = new StringBuilder();
            foreach (var format in m_defaultFormats)
            {
                formats.Append(format.ToString());
                formats.Append(",");
            }

            return formats.ToString();
        }

        protected override void ForEachCheckConfigPath(string path, ConfigCell cell, ReportInfo reportInfo)
        {
            m_defaultFormats.Clear();
            var formats = cell.TryGetAllFiled(FORMAT_FIELD_NAME);
            Debug.Log("自定义TextureImporterFormat数量：" + formats.Count);
            foreach (var param in formats)
            {
                if (Enum.TryParse<TextureImporterFormat>(param.Value, out var format))
                {
                    m_defaultFormats.Add(format);
                }
            }

            if (m_defaultFormats.Count == 0)
            {
                m_defaultFormats.Add(TextureImporterFormat.ASTC_6x6);
            }
            else
            {
                Debug.Log("用自定义检查格式：" + GetFormatsString());
            }

            base.ForEachCheckConfigPath(path, cell, reportInfo);
        }

        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            string formats = GetFormatsString();
            asset = null;
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                asset = AssetDatabase.LoadAssetAtPath<Texture>(path);
                TextureImporterPlatformSettings androidPlatformSetting = textureImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings iosPlatformSetting = textureImporter.GetPlatformTextureSettings("iPhone");
                Debug.Log(androidPlatformSetting.format);
                bool error = false;
                if (!m_defaultFormats.Contains(androidPlatformSetting.format))
                {
                    error = true;
                    reportInfo.AddAssetError(path,asset, string.Format("Android压缩格式 : {0} Android:{1}  正确格式可为:{2}", path, androidPlatformSetting.format.ToString(), formats),ECheckResult.Error,cell);
                }
                if (!m_defaultFormats.Contains(iosPlatformSetting.format))
                {
                    error = true;
                    reportInfo.AddAssetError(path,asset, string.Format("IOS压缩格式 : {0} IOS:{1}  正确格式可为:{2}", path, iosPlatformSetting.format.ToString(), formats),ECheckResult.Error,cell);
                }
                if (error)
                {
                    return ECheckResult.CustomAddError;
                }
                else
                {
                    return ECheckResult.Pass;
                }
                // Resources.UnloadAsset(curTexture);
            }
            else
            {
                return ECheckResult.Warning;
            }
            
        }
    }
}
