using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
        
        public static Dictionary<string, ITreeView> GetMenuTrees()
        {
            Dictionary<string,ITreeView> menuTrees = new Dictionary<string,ITreeView>();
            menuTrees.Add(BASIC_SETTING,new CommonNodeView());
            menuTrees.Add(BASIC_ASSET_SETTING,new BasicAssetTreeView());
            menuTrees.Add(CUSTOM_SETTING,new CustomTreeView());
            
            return menuTrees;
        }
        
        public static UCheckSetting GetConfig()
        {
            if (s_setting == null)
            {
                if (File.Exists(CONFIG_PATH))
                {
                    s_setting = Newtonsoft.Json.JsonConvert.DeserializeObject<UCheckSetting>(File.ReadAllText(CONFIG_PATH));
                }
                else
                {
                    UCheckSetting setting = new UCheckSetting();
                    setting.GlobalDefaultPaths = new List<ConfigCell>(){new ConfigCell("Assets")};
                    setting.GlobalWhiteListPaths = new List<string>();
                    setting.CommonChecks = new List<CommonCheck>();
                    setting.CustomChecks = new List<CommonCheck>();
                    var textureSizeCheck = new CommonCheck(typeof(CommonTextureSizeCheck))
                    {
                        Setting =
                        {
                            Title = "检查纹理尺寸",
                            Rule = "检查纹理尺寸 推荐纹理尺寸为 512*512,如果512*512显示效果够用，就不用1024*1024,默认检查值512",
                            EnableCheck = true,
                            EnableFix = false
                        }
                    };

                    setting.CommonChecks.Add(textureSizeCheck);
                    
                    var customCheck = new CommonCheck(typeof(TestCustomCheck))
                    {
                        Setting =
                        {
                            Title = "自定义检查",
                            Rule = "自定义检查规则",
                            EnableCheck = true,
                            EnableFix = false
                        }
                    };
                    
                    setting.CustomChecks.Add(customCheck);
                    s_setting = setting;
                    SaveConfig();
                }
            }
            return s_setting;
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