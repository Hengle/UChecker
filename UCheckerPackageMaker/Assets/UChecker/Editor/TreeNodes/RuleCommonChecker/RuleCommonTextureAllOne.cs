﻿using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    /// <summary>
    /// 检查纯透明纹理 
    /// </summary>
    [RuleCheck("包含无效透明通道纹理检查","检测到Alpha通道数值都是1，导入时可去掉import alpha选项，节省内存",997)]
    public class RuleCommonTextureAllOne: BaseCommonCheck
    {
        protected override string[] SearchPattern => new[] { "*.png", "*.jpg", "*.tga" };
        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            asset = null;
            var texture  = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture!=null)
            {
                asset = texture;
                if (IsPureNotTransparent(texture))
                {
                    if (texture.alphaIsTransparency)
                    {
                        reportInfo.AddAssetError(path, asset,$"非透明图片 alphaIsTransparency 去掉勾选 : {path}",ECheckResult.Error);
                    }
                }
            }
            return ECheckResult.CustomAddError;
        }
        
        public static bool IsPureNotTransparent(Texture2D texture)
        {
            // TODO 这里接口需要修改
            return false;
            Color[] pixels = texture.GetPixels();
            foreach (Color pixel in pixels)
            {
                if (!Mathf.Approximately(pixel.a, 1))
                    return false;
            }
            return true;
        }
        
        public static bool IsPureTransparent(Texture2D texture)
        {
            // TODO 这里接口需要修改
            return false;
            Color[] pixels = texture.GetPixels();
            foreach (Color pixel in pixels)
            {
                if (!Mathf.Approximately(pixel.a, 0))
                    return false;
            }
            return true;
        }
        
    }
}