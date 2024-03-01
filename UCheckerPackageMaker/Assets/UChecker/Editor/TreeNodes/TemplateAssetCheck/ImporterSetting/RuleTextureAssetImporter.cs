using System.IO;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor.ImporterSetting
{
    [RuleCheck(ERuleCategory.Template, "Texture模板设置","通过模板检查纹理是否满足需求",998)]
    public class RuleTextureAssetImporter:BaseCommonCheck
    {
        private TextureImporter m_textureImporter;
        protected override string[] SearchPattern { get; } = { "*.png", "*.jpg", "*.tga" };

        public override ReportInfo CheckAndFix(CommonCheck ctx)
        {
            if(!string.IsNullOrEmpty(ctx.Setting.TemplateAssetPath) && File.Exists(ctx.Setting.TemplateAssetPath))
            {
                m_textureImporter = AssetImporter.GetAtPath(ctx.Setting.TemplateAssetPath) as TextureImporter;
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
            asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            return ECheckResult.Pass;
        }
    }
}