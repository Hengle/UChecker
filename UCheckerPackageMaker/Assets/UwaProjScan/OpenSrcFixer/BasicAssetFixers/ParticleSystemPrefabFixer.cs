using UnityEngine;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;
using UnityEditor;
using System;
namespace UwaProjScan.ScanRuleFixer
{
    public class ParticleSystemPrefabCollisionFixer : IRuleFixer<RuleID.Prefab.PS_CollisionOrTrigger>
    {
        // Start is called before the first frame update
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
                ParticleSystem[] psArray = Instance.GetComponentsInChildren<ParticleSystem>(true);

                foreach (var ps in psArray)
                {
                    string prefabTree = Utils.PrefabUtils.GetFullHierachyString(ps.gameObject);

                    if (prefabTree == hierachy)
                    {
                        var coll = ps.collision;
                        coll.enabled = false;

                        var trigger = ps.trigger;
                        trigger.enabled = false;
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
