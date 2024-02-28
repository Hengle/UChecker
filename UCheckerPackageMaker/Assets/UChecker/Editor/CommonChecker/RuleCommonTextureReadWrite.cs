using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [RuleCheck("开启Read/Write的纹理","Read/Write的纹理开启后，系统会有有份纹理数据的副本，占用额外内存，双倍内存消耗",999)]
    public class RuleCommonTextureReadWrite: BaseCommonCheck
    {
        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            asset = null;
            var texture  = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture!=null)
            {
                asset = texture;
                if (texture.isReadable)
                {
                    reportInfo.AddAssetError(asset,path);
                }
            }
            return ECheckResult.CustomAdd;
        }
    }
}