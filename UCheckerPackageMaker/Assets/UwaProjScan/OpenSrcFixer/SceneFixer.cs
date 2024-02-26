using System;
using UwaProjScan;
using UnityEngine;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.SceneCheck;
using UnityEditor;
namespace UwaProjScan.ScanRuleFixer
{
    class Scene_StaticRigidBody : IRuleFixer<RuleID.Scene_StaticRigidBody>
    {
        public bool Fix(string hierachy)
        {
            Rigidbody rigidbody;
            GameObject go = GameObject.Find(hierachy);
            rigidbody = go.GetComponent<Rigidbody>();

            if (rigidbody)
                UnityEngine.Object.DestroyImmediate(rigidbody);
            return true;
        }
    }
    class ShadowSolution: IRuleFixer<RuleID.Scene_ShadowResolution>
    {
        public bool Fix(string hierachy)
        {
            try
            {
                Light light = new Light();
                GameObject go = GameObject.Find(hierachy);
                light = go.GetComponent<Light>();
                light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.FromQualitySettings;
            
            }catch(Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
            return true;
        }
    }

    class CameraRenderingPath : IRuleFixer<RuleID.Scene_RenderingPath>
    {
        public bool Fix(string hierachy)
        {
            Camera cam = new Camera();
            GameObject go = GameObject.Find(hierachy);
            cam = go.GetComponent<Camera>();

            cam.renderingPath = RenderingPath.UsePlayerSettings;

            return true;
        }
    }

    class Scene_RealtimeGI : IRuleFixer<RuleID.Scene_RealtimeGI>
    {
        public bool Fix(string Nothing)
        {
            Lightmapping.realtimeGI = false;
            return true;
        }
    }
    class MobileFog : IRuleFixer<RuleID.Editor_MobileFog>
    {
        public bool Fix(string Nothing)
        {
            RenderSettings.fog = false;
            return true;
        }
    }

}
