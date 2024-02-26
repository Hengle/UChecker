using UnityEditor;
using System.Reflection;
using System;
using UnityEngine;
using RuleID = UwaProjScan.ScanRuleFixer.Rule.ProjectAssets;

namespace UwaProjScan.ScanRuleFixer
{
    class Audio_Streaming : IRuleFixer<RuleID.AudioClip.Audio_Streaming>
    {
        public bool Fix(string path)
        {
            string platform = Utils.Tools.GetCurrentPlatform();
            try
            {
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                var audioSampleSetting = Utils.Audio.GetAudioSampleSetting(importer);
                audioSampleSetting.loadType = AudioClipLoadType.Streaming;

                if (importer.ContainsSampleSettingsOverride(platform))
                {
                    importer.SetOverrideSampleSettings(platform, audioSampleSetting);
                }
                else
                {
                    importer.defaultSampleSettings = audioSampleSetting;
                }

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

    class Audio_Format : IRuleFixer<RuleID.AudioClip.Audio_FormatPCM>
    {
        public bool Fix(string path)
        {
            string platform = Utils.Tools.GetCurrentPlatform();
            try
            {
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

                var audioSampleSetting = Utils.Audio.GetAudioSampleSetting(importer);
                audioSampleSetting.compressionFormat = AudioCompressionFormat.PCM;


                if (importer.ContainsSampleSettingsOverride(platform))
                {
                    importer.SetOverrideSampleSettings(platform, audioSampleSetting);
                }
                else
                {
                    importer.defaultSampleSettings = audioSampleSetting;
                }

                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return true;

        }
    }

    class OptimizeSampleRate : IRuleFixer<RuleID.AudioClip.Audio_OptimizeSampleRate>
    {
        public bool Fix(string path)
        {
            string platform = Utils.Tools.GetCurrentPlatform();
            try
            {
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

                var audioSampleSetting = Utils.Audio.GetAudioSampleSetting(importer);
                audioSampleSetting.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;


                if (importer.ContainsSampleSettingsOverride(platform))
                {
                    importer.SetOverrideSampleSettings(platform, audioSampleSetting);
                }
                else
                {
                    importer.defaultSampleSettings = audioSampleSetting;
                }

                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return true;

        }
    }
    class DoubleChannels : IRuleFixer<RuleID.AudioClip.Audio_DoubleChannels>
    {
        public bool Fix(string path)
        {
            try
            {
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

                importer.forceToMono = true;

                importer.SaveAndReimport();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return true;

        }
    }


}
