using System;
using UnityEngine;
using UnityEditor;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;

namespace UwaProjScan.ScanRuleFixer
{
    class AnimationClipResampleCuve : IRuleFixer<RuleID.Animation.AnimationClip_ResampleCurve>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.resampleCurves = false;
                importer.SaveAndReimport();

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    class AnimationClipCompression : IRuleFixer<RuleID.Animation.AnimationClip_Compression>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.animationCompression = ModelImporterAnimationCompression.Optimal;
                
                importer.SaveAndReimport();

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }
}
