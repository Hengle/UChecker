using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UChecker.Editor
{
    [RuleCheck("预制体Shader引用检查","Prefab 引用的Shader在设置范围内,添加参数Shader 和Shader名字",996)]
    public class RulePrefabShaderRefCheck : BaseCommonCheck
    {
        protected override string[] SearchPattern { get; } = new string[] {"*.prefab"};
        private List<string> matchShaders = new List<string>();
        private HashSet<string> errorShaders = new HashSet<string>();
        protected override void ForEachCheckConfigPath(string path, ConfigCell cell, ReportInfo reportInfo)
        {
            matchShaders.Clear();
            List<CellParam> shaderNames = cell.TryGetAllFiled("Shader");
            if (shaderNames.Count == 0)
            {
                reportInfo.AddAssetError(path, AssetDatabase.LoadAssetAtPath<Object>(path), $"预制体Shader引用检查 没有参数Shader Name配置路径：{path}",ECheckResult.Warning,cell);
                return;
            }
            foreach (var cellParam in shaderNames)
            {
                matchShaders.Add(cellParam.Value);
            }
            base.ForEachCheckConfigPath(path, cell, reportInfo);
        }
        protected override ECheckResult ForEachCheckAssetPath(string path, ConfigCell cell, ReportInfo reportInfo, out Object asset)
        {
            errorShaders.Clear();
            asset = null;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var renders =  prefab.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                foreach (var renderSharedMaterial in render.sharedMaterials)
                {
                    if (renderSharedMaterial!=null)
                    {
                        var shader = renderSharedMaterial.shader;
                        if (shader!=null)
                        {
                            if (!matchShaders.Contains(shader.name))
                            {
                                errorShaders.Add(shader.name);
                            }                  
                        }
                    }
                }
            }
            var depends = AssetDatabase.GetDependencies(path);
            foreach (var depend in depends)
            {
                var fileInfo = new FileInfo(depend);
                if (fileInfo.Extension.Equals(".shader"))
                {
                    Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(depend);
                    asset = shader;
                    if (!matchShaders.Contains(shader.name))
                    {
                        errorShaders.Add(shader.name);
                    }  
                }
            }

            foreach (var item in errorShaders)
            {
                reportInfo.AddAssetError(path,prefab,$"{path} 依赖项Shader错误：{item}",ECheckResult.Error,cell);
            }

            bool hasError = errorShaders.Count > 0;
            return hasError ? ECheckResult.CustomAddError : ECheckResult.None;
        }
    }
}