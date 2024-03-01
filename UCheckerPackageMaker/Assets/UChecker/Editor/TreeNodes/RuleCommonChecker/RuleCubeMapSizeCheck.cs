using UnityEditor;

using UnityEngine;

namespace UChecker.Editor
{
    [RuleCheck("CubeMap尺寸大小检查","检查CubeMap尺寸 推荐纹理尺寸为 小于 512*512,如果512*512显示效果够用，就不用1024*1024,默认检查值512\"",995)]
    public class RuleCubeMapSizeCheck : BaseCommonCheck
    {
        public const string FIELD_SIZE_NAME = "size";
        protected override string[] SearchPattern { get; } = new string[] { "*.exr" };
         public int MaxSize = 512;
        protected override void ForEachCheckConfigPath(string path, ConfigCell cell, ReportInfo reportInfo)
        {
            var param = cell.TryGetFiled("size");
            if (param != null && int.TryParse(param.Value, out int v))
            {
                MaxSize = v;
            }
            else
            {
                MaxSize = 512;
            }
            base.ForEachCheckConfigPath(path, cell, reportInfo);
        }

        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            asset = null;
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                TextureImporterPlatformSettings androidPlatformSetting = textureImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings iosPlatformSetting = textureImporter.GetPlatformTextureSettings("iPhone");
                TextureImporterPlatformSettings standalonePlatformSetting = textureImporter.GetPlatformTextureSettings("Standalone");
                int maxTextureSize = Mathf.Max(textureImporter.maxTextureSize, androidPlatformSetting.overridden ? androidPlatformSetting.maxTextureSize : 0,
                    iosPlatformSetting.overridden ? iosPlatformSetting.maxTextureSize : 0, standalonePlatformSetting.overridden ? standalonePlatformSetting.maxTextureSize : 0);
                Texture curTexture = AssetDatabase.LoadAssetAtPath<Texture>(path);
                if (curTexture == null)
                {
                    Debug.Log("Null Path : " + path);
                    return ECheckResult.None;
                }

                int texSize = Mathf.Max(curTexture.width, curTexture.height);
                if (maxTextureSize > texSize)
                {
                    maxTextureSize = texSize;
                }

                if (maxTextureSize > MaxSize)
                {
                    reportInfo.AddAssetError(path, curTexture, string.Format("Too Big CubeMap : {0} 当前尺寸为:{1} + 合理尺寸为:{2}", path, maxTextureSize, MaxSize),ECheckResult.Error,cell);
                }
                asset = curTexture;
                return ECheckResult.CustomAddError;
                // Resources.UnloadAsset(curTexture);
            }
            else
            {
                return ECheckResult.Warning;
            }
        }
    }
}
