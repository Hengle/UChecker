using UnityEditor;
using System;
using UnityEngine;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;

namespace UwaProjScan.ScanRuleFixer
{
    //Provider Api:
    //Utils.GetPlatformTextureSetting(TextureImporter);
    class Texture_AlphaAllOne : IRuleFixer<RuleID.Texture2D.Texture_AlphaAllOne>
    {
        public bool Fix(string path)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                var textureSetting = Utils.Texture.GetPlatformTextureSetting(importer);
                importer.alphaSource = TextureImporterAlphaSource.None;

                if (textureSetting.format != TextureImporterFormat.Automatic)
                {
                    textureSetting.format = TextureImporterFormat.Automatic;
                    importer.SetPlatformTextureSettings(textureSetting);
                }
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Texture_CompressionFormat : IRuleFixer<RuleID.Texture2D.Texture_CompressionFormat>
    {
        public bool Fix(string path)
        {

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            var textSetting = Utils.Texture.GetPlatformTextureSetting(importer);

            textSetting.textureCompression = TextureImporterCompression.Compressed;
            importer.SetPlatformTextureSettings(textSetting);
            importer.SaveAndReimport();
            return true;
        }

    }

    
    class Texture_FilterMode : IRuleFixer<RuleID.Texture2D.Texture_FilterMode>
    { 
        public bool Fix(string path)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Texture_RW : IRuleFixer<RuleID.Texture2D.Texture_RW>
    {
        public bool Fix(string path)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.isReadable = false;
                
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Texture_SpriteMipmap : IRuleFixer<RuleID.Texture2D.Texture_SpriteMipmap>
    {
        public bool Fix(string path)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Texture_WrapMode : IRuleFixer<RuleID.Texture2D.Texture_WrapMode>
    {
        public bool Fix(string path)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }
}
