using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    public class CommonTextureSizeCheck : BaseCommonCheck
    {
        public int MaxSize = 512;
        protected override string[] SearchPattern => new[] { "*.png", "*.jpg", "*.tga" };

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
                    reportInfo.AddAssetError(curTexture, string.Format("Too Big Texture : {0} 当前尺寸为:{1} + 合理尺寸为:{2}", path, maxTextureSize, MaxSize));
                }

                asset = curTexture;
                return ECheckResult.CustomAdd;
                // Resources.UnloadAsset(curTexture);
            }
            else
            {
                return ECheckResult.Warning;
            }
        }
    }
}