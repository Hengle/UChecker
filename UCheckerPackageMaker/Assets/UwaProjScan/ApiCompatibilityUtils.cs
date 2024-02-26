using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UwaProjScan.Tools;
using UwaProjScan.Submodules.Shader_Analysis.Build;
using System.IO;
#if UNITY_2018_2_OR_NEWER
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Build.Player;
using UnityEditor.Rendering;
#endif

#if UNITY_2017_3_OR_NEWER
using UnityEditor.Compilation;
#endif


namespace UwaProjScan
{
    class ApiCompatibilityImp : ICompatApi
    {
        public static readonly ApiCompatibilityImp Instance = new ApiCompatibilityImp();
        public ApiCompatibilityImp()
        {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += EditorApplication_playmodeStateChanged;
#else
                EditorApplication.playmodeStateChanged += EditorApplication_playmodeStateChanged;
#endif
        }

        private Action _exitplaymodecb = null;
        private Action<bool> _pausemodecb = null;
        public Action<Shader, UPS_SSD, List<UPS_SCD>> _onProcessShader = null;

        private bool _lastPauseState = false;
#if UNITY_2017_2_OR_NEWER
        public void EditorApplication_playmodeStateChanged(PlayModeStateChange p)
#else
            public void EditorApplication_playmodeStateChanged()
#endif
        {
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (_exitplaymodecb != null) _exitplaymodecb();
            }

            // invoke only when isPaused changed
            if (_lastPauseState != EditorApplication.isPaused)
            {
                if (_pausemodecb != null) _pausemodecb(EditorApplication.isPaused);
            }

            _lastPauseState = EditorApplication.isPaused;
        }

#region interface imp
        public void RegisterEditorExitPlayMode(Action cb)
        {
            _exitplaymodecb += cb;
        }

        public void RegisterEditorPauseMode(Action<bool> cb)
        {
            _pausemodecb += cb;
        }

        public void RegisterProcessShader(Action<Shader, UPS_SSD, List<UPS_SCD>> onProcessShaderInternal)
        {
            _onProcessShader += onProcessShaderInternal;
        }

        public void Reset()
        {
            _exitplaymodecb = null;
            _pausemodecb = null;
            _onProcessShader = null;
        }

        public void UPS_BuildPlayer(string[] scenesInBuild)
        {
#if UNITY_2018_2_OR_NEWER
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenesInBuild;
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildPlayerOptions.locationPathName = "./UWAScan_SA_BuildTest_Standalone/UWAScan_SA_BuildTest.exe";
                    break;
                case BuildTarget.Android:
                    buildPlayerOptions.locationPathName = "./UWAScan_SA_BuildTest_Android";
                    break;
                case BuildTarget.iOS:
                    buildPlayerOptions.locationPathName = "./UWAScan_SA_BuildTest_iOS";
                    break;
                default:
                    buildPlayerOptions.locationPathName = "./UWAScan_SA_Build_Test";
                    break;
            }


            buildPlayerOptions.options = BuildOptions.None;

            BuildPipeline.BuildPlayer(buildPlayerOptions);

#endif
        }

        public string[] GetAssemblyPaths()
        {
            List<string> list = new List<string>();
#if UNITY_2017_3_OR_NEWER

    #if UNITY_2019_3_OR_NEWER
                var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
    #elif UNITY_2018_1_OR_NEWER
                var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
    #elif UNITY_2017_3_OR_NEWER
                var assemblies = CompilationPipeline.GetAssemblies();
    #endif
            foreach(var asm in assemblies)
            {
                list.Add(asm.outputPath);
            }
#endif
            return list.ToArray();
        }

        public string GetAssemblyDefinitionFilePath(string asmName)
        {
            string dfPath = null;
#if UNITY_2017_3_OR_NEWER
            dfPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(asmName);
#endif
            return dfPath;
        }

        public string[] CompileAssemblies(string folder)
        {
#if UNITY_2018_2_OR_NEWER

            var input = new ScriptCompilationSettings
            {
                target = EditorUserBuildSettings.activeBuildTarget,
                group = EditorUserBuildSettings.selectedBuildTargetGroup
            };

            List<string> paths = new List<string>();
            var compilationResult = PlayerBuildInterface.CompilePlayerScripts(input, folder);
            foreach(var asm in compilationResult.assemblies)
            {
                paths.Add(System.IO.Path.Combine(folder, asm));
            }
            return paths.ToArray();
#else
            return null;
#endif
        }

        #endregion

        public string[] GetEnvironmentVariables()
        {
            List<string> dirs = new List<string>();
            List<string> assemblyPaths = new List<string>();
#if UNITY_2019_1_OR_NEWER
            assemblyPaths.AddRange(CompilationPipeline.GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources
                .UserAssembly));
            assemblyPaths.AddRange(CompilationPipeline.GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources
                .UnityEngine));
#elif UNITY_2018_4_OR_NEWER
            assemblyPaths.AddRange(CompilationPipeline.GetPrecompiledAssemblyNames()
                .Select(a => CompilationPipeline.GetPrecompiledAssemblyPathFromAssemblyName(a)));
#endif
#if UNITY_5_6
            assemblyPaths.AddRange(Directory.GetFiles(Path.Combine(EditorApplication.applicationContentsPath,
    "Managed")).Where(path => Path.GetExtension(path).Equals(".dll")));

#else
            assemblyPaths.AddRange(Directory.GetFiles(Path.Combine(EditorApplication.applicationContentsPath,
                Path.Combine("Managed",
                    "UnityEngine"))).Where(path => Path.GetExtension(path).Equals(".dll")));
#endif

#if !UNITY_2019_2_OR_NEWER
            var files = Directory.GetFiles(Path.Combine(EditorApplication.applicationContentsPath, Path.Combine(
                "UnityExtensions",
                Path.Combine("Unity", "GUISystem"))));
            assemblyPaths.AddRange(files.Where(path => Path.GetExtension(path).Equals(".dll")));
#endif
            foreach (var dir in assemblyPaths.Select(path => Path.GetDirectoryName(path.Replace("\\", "/"))).Distinct())
            {
                dirs.Add(dir);
            }
            return dirs.ToArray();
        }

        public bool CheckStrippingManagedCode()
        {
#if UNITY_2018_3_OR_NEWER
        ManagedStrippingLevel androidLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
        ManagedStrippingLevel iosLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.iOS);
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            if (androidLevel == ManagedStrippingLevel.Disabled || androidLevel == ManagedStrippingLevel.Low)
                return false;
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            if (iosLevel == ManagedStrippingLevel.Low)
                return false;
        }
        return true;
#else
        if (PlayerSettings.strippingLevel == StrippingLevel.Disabled) return false;
        else return true;

#endif
        }
    }


#if UNITY_2018_2_OR_NEWER

    class UPS_SA_BuildProcessor : IPreprocessShaders
    {

        public int callbackOrder { get { return 100; } }

        public void OnProcessShader(
            Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerDatas)
        {
            if (!ApiCompatibilityUtils.UwaShaderProcessorOn) return;

            UPS_SSD u_snippet = new UPS_SSD();
            u_snippet.shaderType = (UPS_SSD.UPS_ShaderType)Enum.Parse(typeof(UPS_SSD.UPS_ShaderType), snippet.shaderType.ToString());
            List<UPS_SCD> u_SCDs = new List<UPS_SCD>();

            for (int i = 0; i < shaderCompilerDatas.Count; ++i)
            {
                ShaderKeywordSet ks = shaderCompilerDatas[i].shaderKeywordSet;
                UPS_SCD u_SCD = new UPS_SCD();
                foreach (ShaderKeyword kw in ks.GetShaderKeywords())
                {
                    string kname;
#if UNITY_2019_3_OR_NEWER
                    kname = ShaderKeyword.GetKeywordName(shader, kw);
#elif UNITY_2018_3_OR_NEWER
                        kname = kw.GetKeywordName();
#else
                        kname = kw.GetName();
#endif
                    u_SCD.shaderKeywordSet.Add(kname);
                }
                u_SCDs.Add(u_SCD);

            }

            ApiCompatibilityImp.Instance._onProcessShader.Invoke(shader, u_snippet, u_SCDs);
            shaderCompilerDatas.Clear();
        }
    }
#endif



}