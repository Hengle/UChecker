using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UwaProjScan.Tools;
using UwaProjScan.Submodules.Shader_Analysis.Build;


namespace UwaProjScan
{

    [InitializeOnLoad]
    static class Launcher
    {
        static Launcher()
        {
            ApiCompatibilityUtils.Instance.Setup(ApiCompatibilityImp.Instance);
        }

        [MenuItem("Tools/UWA Scan/Run All Forcedly", false, 1)]
        private static void RunAll()
        {
            Scanner.RunAll();
        }

        [MenuItem("Tools/UWA Scan/Run Seleted Modules", false, 2)]
        private static void Run_Selected_Modules()
        {
            Scanner.Run_Selected_Modules();
        }

        [MenuItem("Tools/UWA Scan/Run Basic Assets Check", false, 3)]
        private static void Run_Basic_Assets_Check()
        {
            Scanner.Run_Basic_Assets_Check();
        }

        [MenuItem("Tools/UWA Scan/Run Scenes Check", false, 4)]
        private static void Run_Scenes_Check()
        {
            Scanner.Run_Scenes_Check();
        }

        [MenuItem("Tools/UWA Scan/Run Global Settings Check", false, 5)]
        private static void Run_Global_Settings_Check()
        {
            Scanner.Run_Global_Settings_Check();
        }

        [MenuItem("Tools/UWA Scan/Run C# Scripts Check", false, 6)]
        private static void Run_CS_Check()
        {
            Scanner.Run_CS_Check();
        }

        [MenuItem("Tools/UWA Scan/Run Lua Scripts Check", false, 7)]
        private static void Run_Lua_Check()
        {
            Scanner.Run_Lua_Check();
        }

        [MenuItem("Tools/UWA Scan/Run Custom Rules", false, 8)]
        private static void Run_Custom_Rules_Check()
        {
            Scanner.Run_Custom_Rules_Check();
        }



        [MenuItem("Tools/UWA Scan/Run Effects Play Check", false, 9)]
        private static void Run_Effects_Play_Check()
        {
            Scanner.Run_Effects_Play_Check();
        }


        [MenuItem("Tools/UWA Scan/Run Shader Analysis", false, 10)]
        private static void Run_Shader_Analysis()
        {
            Scanner.Run_Shader_Analysis();
        }

        [MenuItem("Tools/UWA Scan/Run Art Assets Check", false, 11)]
        private static void Run_Art_Assets_Check()
        {
            Scanner.Run_Art_Assets_Check();
        }
        
        //[MenuItem("Tools/UWA Scan/Run Assets Reference Check", false, 12)]
        //private static void Run_Assets_Reference_Check()
        //{
        //    Scanner.Run_Assets_Reference_Check();
        //}
    }
    static class FixLauncher
    {
        [MenuItem("Tools/UWA Scan/Fix Resource/Fix All", false, 21)]
        private static void FixAll()
        {
            FixerRunner.FixAll();
        }

        [MenuItem("Tools/UWA Scan/Fix Resource/Fix Selected Modules", false, 21)]
        private static void FixSelectedModule()
        {
            FixerRunner.FixSelectedModules();
        }
        [MenuItem("Tools/UWA Scan/Fix Resource/Fix Global Setting", false, 22)]
        private static void FixGlobalSettings()
        {
            FixerRunner.FixGlobalSettings();
        }

        [MenuItem("Tools/UWA Scan/Fix Resource/Fix Basic Assets", false, 23)]
        private static void FixBasicAssets()
        {
            FixerRunner.FixBasicAssets();
        }

        [MenuItem("Tools/UWA Scan/Fix Resource/Fix Scenes", false, 24)]
        private static void FixScene()
        {
            FixerRunner.FixScenes();
        }
    }
    
}
