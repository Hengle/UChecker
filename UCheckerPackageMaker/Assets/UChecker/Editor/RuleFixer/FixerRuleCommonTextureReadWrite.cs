using UnityEditor;
using UnityEngine;

namespace UChecker.Editor.RuleFixer
{
    public class FixerRuleCommonTextureReadWrite : IRuleFixer<RuleCommonTextureReadWrite>
    {
        public bool Fix(string path, ECheckResult result, RuleCommonTextureReadWrite check, ConfigCell cell)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter!=null)
            {
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
            }
            return true;
        }
    }
}