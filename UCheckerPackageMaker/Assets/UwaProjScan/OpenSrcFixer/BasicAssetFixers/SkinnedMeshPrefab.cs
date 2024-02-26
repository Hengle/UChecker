using System.Collections;
using System.Collections.Generic;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;
using UnityEngine;
using UnityEditor;
using System;
namespace UwaProjScan.ScanRuleFixer
{
    class MotionVectorFixer : IRuleFixer<RuleID.Prefab.SkinnedMeshRenderer_MotionVector>
    {
        public bool Fix(string path)
        {
            string[] data = path.Split('$');
            string assetPath = data[0];
            string hierachy = string.Empty;

            if (data.Length > 1)
            {
                hierachy = data[1];
            }

            try
            {

                UnityEngine.Object Obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;

                GameObject Instance = GameObject.Instantiate(Obj as GameObject);
                SkinnedMeshRenderer[] comArray = Instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                foreach (var skinMeshRenderer in comArray)
                {
                    string prefabTree = Utils.PrefabUtils.GetFullHierachyString(skinMeshRenderer.gameObject);
                    if (prefabTree == hierachy)
                    {
                        skinMeshRenderer.skinnedMotionVectors = false;
                        break;
                    }
                }


                PrefabUtility.ReplacePrefab(Instance, Obj);

                //Remember to always destroy the instance after saving it to desk!
                GameObject.DestroyImmediate(Instance);

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
