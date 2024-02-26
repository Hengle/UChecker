using UnityEngine;
using UnityEditor;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.EditorSettings;
namespace UwaProjScan.ScanRuleFixer
{
    class Quality_GlobalAnisoFilter : IRuleFixer<RuleID.Quality_GlobalAnisoFilter>
    {
        public bool Fix(string Nothing)
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == (BuildTarget)9)
            {
               QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            }
            else
            {
                Debug.Log("The AnisoFiler will only be fixed while in Mobile Platform");
                return false;
            }
            return true;
        }
    }


    class Player_CodeStripping : IRuleFixer<RuleID.Player_CodeStripping>
    { 
        public bool Fix(string Nothion)
        {
            PlayerSettings.stripEngineCode = true;
            return true;
        }
    }

    class Player_OptimizeMeshData : IRuleFixer<RuleID.Player_OptimizeMeshDataDisabled>
    {
        public bool Fix(string Nothion)
        {
            PlayerSettings.stripUnusedMeshComponents = true;
            return true;
        }
    }

}
