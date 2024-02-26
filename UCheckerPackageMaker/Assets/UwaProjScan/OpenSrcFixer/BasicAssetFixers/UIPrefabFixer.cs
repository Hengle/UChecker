using System;
using UnityEngine;
using UnityEditor;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;

namespace UwaProjScan.ScanRuleFixer
{

    class ImageUnTiled : IRuleFixer<RuleID.Prefab.UIImage_Tiled>
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
                UnityEngine.UI.Image[] imageArray = Instance.GetComponentsInChildren<UnityEngine.UI.Image>(true);

                foreach (var image in imageArray)
                {
                    string prefabTree = Utils.PrefabUtils.GetFullHierachyString(image.gameObject);

                    if (prefabTree == hierachy)
                    {
                        image.type = UnityEngine.UI.Image.Type.Simple;
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
