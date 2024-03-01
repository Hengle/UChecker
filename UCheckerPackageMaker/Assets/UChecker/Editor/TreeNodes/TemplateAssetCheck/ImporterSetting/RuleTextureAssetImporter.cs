using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [RuleCheck(ERuleCategory.Template, "Texture模板设置","通过模板检查纹理是否满足需求",998)]
    public class RuleTextureAssetImporter:BaseCommonCheck
    {
        private TextureImporterSettings m_templateSetting = new TextureImporterSettings();

        private TextureImporter m_textureImporter;
        protected override string[] SearchPattern { get; } = { "*.png", "*.jpg", "*.tga" };
        private List<bool> settings = new List<bool>();

        public override ReportInfo CheckAndFix(CommonCheck ctx)
        {
            if(!string.IsNullOrEmpty(ctx.Setting.TemplateAssetPath) && File.Exists(ctx.Setting.TemplateAssetPath))
            {
                m_textureImporter = AssetImporter.GetAtPath(ctx.Setting.TemplateAssetPath) as TextureImporter;
                if (m_textureImporter!=null)
                {
                    m_textureImporter.ReadTextureSettings(m_templateSetting);
                }
            }
            return base.CheckAndFix(ctx);
        }

        protected override void ForEachCheckConfigPath(string path, ConfigCell cell, ReportInfo reportInfo)
        {
            if (m_textureImporter==null)
            {
                return;
            }
            base.ForEachCheckConfigPath(path, cell, reportInfo);
        }
        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            Debug.Log(path);
            asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer!=null)
            {
                TextureImporterSettings assetSetting = new TextureImporterSettings();
                m_textureImporter.ReadTextureSettings(assetSetting);
                // 检查一列？
                // settings.Add( assetSetting.alphaSource ==  m_templateSetting.alphaSource );
                // settings.Add( assetSetting.mipmapFilter ==  m_templateSetting.mipmapFilter );
                // settings.Add( assetSetting.mipmapEnabled ==  m_templateSetting.mipmapEnabled );
                // settings.Add( assetSetting.sRGBTexture ==  m_templateSetting.sRGBTexture );
                // settings.Add( assetSetting.fadeOut ==  m_templateSetting.fadeOut );
                // settings.Add( assetSetting.borderMipmap ==  m_templateSetting.borderMipmap );
                // settings.Add( assetSetting.mipMapsPreserveCoverage ==  m_templateSetting.mipMapsPreserveCoverage );
                // settings.Add( assetSetting.alphaTestReferenceValue ==  m_templateSetting.alphaTestReferenceValue );
                // settings.Add( assetSetting.mipmapFadeDistanceStart ==  m_templateSetting.mipmapFadeDistanceStart );
                // settings.Add( assetSetting.mipmapFadeDistanceEnd ==  m_templateSetting.mipmapFadeDistanceStart );
                // settings.Add( assetSetting.convertToNormalMap ==  m_templateSetting.convertToNormalMap );
                // settings.Add( assetSetting.heightmapScale ==  m_templateSetting.heightmapScale );
                // settings.Add( assetSetting.normalMapFilter ==  m_templateSetting.normalMapFilter );
                // settings.Add( assetSetting.singleChannelComponent ==  m_templateSetting.singleChannelComponent );
                // settings.Add( assetSetting.flipbookRows ==  m_templateSetting.flipbookRows );
                
                importer.SetTextureSettings(m_templateSetting);
                importer.SaveAndReimport(); 
                // 上报或自动设置？ 考虑下 TODO
            }
            return ECheckResult.Pass;
        }
    }
}