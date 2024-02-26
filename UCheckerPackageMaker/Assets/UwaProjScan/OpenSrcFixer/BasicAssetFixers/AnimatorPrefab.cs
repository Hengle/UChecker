using System;
using UnityEngine;
using UnityEditor;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;

namespace UwaProjScan.ScanRuleFixer
{

    class UnApplyRootMotion : IRuleFixer<RuleID.Prefab.Animator_ApplyRootMotion>
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

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
                if (obj == null) { return false; }

                GameObject Instance = GameObject.Instantiate(obj as GameObject);
                Animator[] animatorArray = Instance.GetComponentsInChildren<Animator>(true);

                foreach (var animator in animatorArray)
                {
                    string prefabTree = Utils.PrefabUtils.GetFullHierachyString(animator.gameObject);

                    if (prefabTree == hierachy)
                    {
                        animator.applyRootMotion = false;
                        break;
                    }
                }


                PrefabUtility.ReplacePrefab(Instance, obj);

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
