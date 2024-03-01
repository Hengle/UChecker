using System;
using UnityEditor;

namespace UChecker.Editor
{
    public class AssetProcessor : AssetPostprocessor
    {
        [NonSerialized]
        public const string IOS = "iPhone";
        [NonSerialized]
        public const string Android = "Android";
        [NonSerialized]
        public const string Standalone = "Standalone";
        
        public void OnPreprocessTexture()
        {
            OnProcessAsset(typeof(TextureImporter));
        }
        private void OnProcessAsset(Type importer)
        {
            
        }
    }
}