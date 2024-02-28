using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace UChecker.Editor
{
    public enum ECheckResult
    {
        None,
        Pass,
        Error,
        Warning,
        CustomAdd,
    }
    public class UCheckSetting
    {
        public List<ConfigCell> GlobalDefaultPaths;
        public List<string> GlobalWhiteListPaths;
        public List<CommonCheck> CommonChecks;
        public List<CommonCheck> CustomChecks;
    }

    // 配置系统
    public static class UCheckConfig
    {
        private static UCheckSetting s_setting;
        public const string BASIC_SETTING = "基本设置";
        public const string BASIC_ASSET_SETTING = "基本资源检查";
        public const string CUSTOM_SETTING = "自定义检查";
        public const string CONFIG_PATH = "Assets/UChecker/CheckConfig.json";
        public const string NAME_SPACE = "UChecker.Editor";
        public static Dictionary<string, ITreeView> GetMenuTrees()
        {
            Dictionary<string,ITreeView> menuTrees = new Dictionary<string,ITreeView>();
            menuTrees.Add(BASIC_SETTING,new BasicSettingNodeView());
            menuTrees.Add(BASIC_ASSET_SETTING,new BasicAssetTreeView());
            menuTrees.Add(CUSTOM_SETTING,new CustomTreeView());
            
            return menuTrees;
        }
        
        public static UCheckSetting GetConfig()
        {
            if (s_setting == null)
            {
                UCheckSetting setting = new UCheckSetting();
                setting.GlobalDefaultPaths = new List<ConfigCell>(){new ConfigCell("Assets")};
                setting.GlobalWhiteListPaths = new List<string>();
                setting.CommonChecks = new List<CommonCheck>();
                setting.CustomChecks = new List<CommonCheck>();
                AddBasicCheckSetting(setting.CommonChecks);
                
                if (File.Exists(CONFIG_PATH))
                {
                    var localConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UCheckSetting>(File.ReadAllText(CONFIG_PATH));
                    MergeUCheckSetting(setting, localConfig);
                    SaveConfig();
                }
                s_setting = setting;
                SaveConfig();
            }
            
            return s_setting;
        }

        private static void MergeUCheckSetting(UCheckSetting target, UCheckSetting input)
        {
            target.GlobalDefaultPaths = input.GlobalDefaultPaths;
            target.GlobalWhiteListPaths = input.GlobalWhiteListPaths;
            Debug.Log("input from local CommonChecks: " +input.CommonChecks.Count);
            target.CommonChecks.RemoveAll(t => input.CommonChecks.Exists(f=>t.Setting.Title.Equals(f.Setting.Title)));
            target.CommonChecks.AddRange(input.CommonChecks);
            Debug.Log("input from local CustomChecks: " +input.CustomChecks.Count);
            target.CustomChecks.RemoveAll(t => input.CustomChecks.Exists(f=>t.Setting.Title.Equals(f.Setting.Title)));
            target.CustomChecks.AddRange(input.CustomChecks);
        }
        
        private static void AddBasicCheckSetting(List<CommonCheck> checks)
        {
            var doMain = AppDomain.CurrentDomain;
            // var assemblies = doMain.ReflectionOnlyGetAssemblies(); 也可以通过反射
            var assemblies = doMain.GetAssemblies();
            // 遍历，所有的程序集
            for (int i = 0; i < assemblies.Length; i++)
            {
                //TODO 此处可以过滤掉不需要的程序集，提升效率
                var assembly = assemblies[i];
                var types = assembly.GetTypes();
                // 遍历程序集内所有的类型
                for (int j = 0; j < types.Length; j++)
                {
                    var typeNameSpace = types[j].Namespace;
                    // 过滤命名空间，提升效率
                    if (string.IsNullOrEmpty(typeNameSpace) || !typeNameSpace.Equals(NAME_SPACE))
                    {
                        continue;
                    }

                    var targetType = types[j];
                    // 过滤掉泛型， 可有可无
                    if (targetType.IsGenericTypeDefinition)
                        continue;
                    var attr = targetType.GetCustomAttribute<CommonCheckAttribute>();
                    if (attr == null)
                        continue;
                    // 此处识别到了添加了指定自定义属性的Type，以及属性中的值
                    string tile = attr.title;
                    string rule = attr.rule;
                    bool enableCheck = attr.enableCheck;
                    bool enableFix = attr.enableFix;
                    var check = new CommonCheck(targetType)
                    {
                        Setting =
                        {
                            Title = tile,
                            Rule = rule,
                            EnableCheck = enableCheck,
                            EnableFix = enableFix,
                            Priority = attr.priority
                        }
                    };
                    checks.Add(check);
                }
            }
            
            checks.Sort((x,y)=>x.Setting.Priority.CompareTo(y.Setting.Priority));
            
              
            // var textureSizeCheck = new CommonCheck(typeof(CommonTextureSizeCheck))
            // {
            //     Setting =
            //     {
            //         Title = "检查纹理尺寸",
            //         Rule = "检查纹理尺寸 推荐纹理尺寸为 512*512,如果512*512显示效果够用，就不用1024*1024,默认检查值512",
            //         EnableCheck = true,
            //         EnableFix = false
            //     }
            // };
            // var textureFormatSizeCheck = new CommonCheck(typeof(CommonTextureFormatCheck))
            // {
            //     Setting =
            //     {
            //         Title = "检查移动平台纹理压缩格式",
            //         Rule = "检查纹理压缩格式 默认压缩格式 ASTC6x6 需要其它压缩格式可添加Format多个字段",
            //         EnableCheck = true,
            //         EnableFix = false
            //     }
            // };
            // var textureAllOne = new CommonCheck(typeof(CommonTextureAllOne))
            // {
            //     Setting =
            //     {
            //         Title = "包含无效透明通道纹理检查",
            //         Rule = "检测到Alpha通道数值都是1，导入时可去掉import alpha选项，节省内存",
            //         EnableCheck = true,
            //         EnableFix = false
            //     }
            // };
            // var textureReadWrite = new CommonCheck(typeof(CommonTextureReadWrite))
            // {
            //     Setting =
            //     {
            //         Title = "开启Read/Write的纹理",
            //         Rule = "Read/Write的纹理开启后，系统会有有份纹理数据的副本，占用额外内存，双倍内存消耗",
            //         EnableCheck = true,
            //         EnableFix = false
            //     }
            // };
            // checks.Add(textureSizeCheck);
            // checks.Add(textureFormatSizeCheck);
            // checks.Add(textureAllOne);
            // checks.Add(textureReadWrite);
        }

        public static void SaveConfig()
        {
            if (s_setting != null)
            {
                File.WriteAllText(CONFIG_PATH,Newtonsoft.Json.JsonConvert.SerializeObject(s_setting,Formatting.Indented));
            }
        }
    }
    
}