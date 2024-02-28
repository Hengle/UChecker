using UnityEngine;

namespace UChecker.Editor.RuleFixer
{
    public class FixerRuleCommonTextureFormat : IRuleFixer<RuleCommonTextureFormatCheck>
    {
        public bool Fix(string path, ECheckResult result, RuleCommonTextureFormatCheck check, ConfigCell cell)
        {
            Debug.Log("Fix Asset :" +path);
            return true;
        }
    }
}