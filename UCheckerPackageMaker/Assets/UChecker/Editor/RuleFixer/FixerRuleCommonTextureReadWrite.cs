using UnityEngine;

namespace UChecker.Editor.RuleFixer
{
    public class FixerRuleCommonTextureReadWrite : IRuleFixer<RuleCommonTextureReadWrite>
    {
        public bool Fix(string path, ECheckResult result, RuleCommonTextureReadWrite check, ConfigCell cell)
        {
            Debug.Log("Fix Asset :" +path);
            return true;
        }
    }
}