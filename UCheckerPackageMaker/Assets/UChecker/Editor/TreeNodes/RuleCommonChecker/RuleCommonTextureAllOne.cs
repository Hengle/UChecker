using System.IO;
using UnityEditor;
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
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                fileStream.Seek(0, SeekOrigin.Begin);
                //创建文件长度缓冲区
                byte[] bytes = new byte[fileStream.Length];
                //读取文件
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                //释放文件读取流
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;
                Texture2D copy= new Texture2D(texture.width, texture.height);
                copy.LoadImage(bytes);
                asset = texture;
                if (IsPureNotTransparent(copy))
                {
                    if (texture.alphaIsTransparency)
                    {
                        reportInfo.AddAssetError(path, asset,$"非透明图片 alphaIsTransparency 去掉勾选 : {path}",ECheckResult.Error,cell);
                    }
                }
                Object.DestroyImmediate(copy);
            }
            return ECheckResult.CustomAddError;
        }
        
        public static bool IsPureNotTransparent(Texture2D texture)
        {
            // TODO 这里接口需要修改
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