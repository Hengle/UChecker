using System;
using UnityEditor;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;
namespace UwaProjScan.ScanRuleFixer.Mesh
{
    public class Mesh_RW : IRuleFixer<RuleID.Mesh.Mesh_RW>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.isReadable = false;
                
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }

            return true;
        }
    }

    
    class Mesh_OptimizeMesh : IRuleFixer<RuleID.Mesh.Mesh_OptimizeMesh>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.optimizeMesh = true;
                importer.SaveAndReimport();
            }catch(Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Mesh_Tangent : IRuleFixer<RuleID.Mesh.Mesh_Tangent>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.importTangents = ModelImporterTangents.None;
                importer.SaveAndReimport();
            }catch(Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }

    
    class Mesh_Normal : IRuleFixer<RuleID.Mesh.Mesh_Normal>
    {
        public bool Fix(string path)
        {
            try
            {
                string[] data = path.Split('$');
                string assetPath = data[0];

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                importer.importNormals = ModelImporterNormals.None;
                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.ToString());
                return false;
            }
            return true;
        }
    }
}
