using System;
using System.Collections.Generic;
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
        CustomAddError,
        CustomAddWarning,
        Fixed
    }

    public class UCheckSetting
    {
        public List<ConfigCell> GlobalDefaultPaths;
        public List<string> GlobalWhiteListPaths;
        
        //这里暂不扩展 可能用dic扩展性高点但会增加不少查找逻辑 不直观
        public List<CommonCheck> CommonChecks;
        public List<CommonCheck> CustomChecks;
    }

    public class TreeViewItem
    {
        public string Name;
        public ITreeView View;
        public int Priority;
    }

    // 配置系统
    public static class UCheckConfig
    {
        private static UCheckSetting s_setting;
        public const string CONFIG_PATH = "Assets/UChecker/CheckConfig.json";
        public const string NAME_SPACE = "UChecker.Editor";
        public const string NAME_SPACE_FIX = "UChecker.Editor.RuleFixer";

        public static readonly string[] AssemblyPaths = new string[]
        {
            // "Assembly-CSharp-Editor",
            "UChecker.Editor",
            // "Assembly-CSharp",
        };
 
        public static List<TreeViewItem> GetMenuTreeItems()
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
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
                    var attr = targetType.GetCustomAttribute<TreeViewAttribute>();
                    if (attr == null)
                        continue;
                    // 此处识别到了添加了指定自定义属性的Type，以及属性中的值
                    items.Add(new TreeViewItem()
                    {
                        Name = attr.name,
                        Priority = attr.priority,
                        View = System.Activator.CreateInstance(targetType) as ITreeView,
                    });
                }
            }

            items.Sort((x, y) => y.Priority.CompareTo(x.Priority));
            return items;
        }

        public static UCheckSetting GetConfig()
        {
            if (s_setting == null)
            {
                UCheckSetting setting = new UCheckSetting();
                setting.GlobalDefaultPaths = new List<ConfigCell>() { new ConfigCell("Assets") };
                setting.GlobalWhiteListPaths = new List<string>();
                setting.CommonChecks = new List<CommonCheck>();
                setting.CustomChecks = new List<CommonCheck>();
                AddBasicCheckSetting(setting.CommonChecks);
                var fixers = ReadFixTypes();
                if (File.Exists(CONFIG_PATH))
                {
                    var localConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UCheckSetting>(File.ReadAllText(CONFIG_PATH));
                    MergeUCheckSetting(setting, localConfig);
                    SaveConfig();
                }
                s_setting = setting;
                MapCheckFixer(s_setting.CommonChecks, fixers);
                MapCheckFixer(s_setting.CustomChecks, fixers);
                SaveConfig();
            }

            return s_setting;
        }

   
        private static void MergeUCheckSetting(UCheckSetting target, UCheckSetting input)
        {
            target.GlobalDefaultPaths = input.GlobalDefaultPaths;
            target.GlobalWhiteListPaths = input.GlobalWhiteListPaths;
            Debug.Log("input from local CommonChecks: " + input.CommonChecks.Count);
            target.CommonChecks.RemoveAll(t => input.CommonChecks.Exists(f => t.Setting.Title.Equals(f.Setting.Title)));
            target.CommonChecks.AddRange(input.CommonChecks);
            Debug.Log("input from local CustomChecks: " + input.CustomChecks.Count);
            target.CustomChecks.RemoveAll(t => input.CustomChecks.Exists(f => t.Setting.Title.Equals(f.Setting.Title)));
            target.CustomChecks.AddRange(input.CustomChecks);
            target.CommonChecks.Sort((x, y) => y.Setting.Priority.CompareTo(x.Setting.Priority));
            target.CustomChecks.Sort((x, y) => y.Setting.Priority.CompareTo(x.Setting.Priority));
        }

        private static void MapCheckFixer(List<CommonCheck> checks,Dictionary<string,string> fixers)
        {
            foreach (var check in checks)
            {
                if (fixers.TryGetValue(check.CheckType,out var fixType))
                {
                    check.FixType = fixType;
                }
                else
                {
                    check.FixType = null;
                }
            }
        }

        
        private static void AddBasicCheckSetting(List<CommonCheck> checks)
        {
            Assembly[] assemblies = new Assembly[AssemblyPaths.Length];
            for (int i = 0; i < AssemblyPaths.Length; i++)
            {
                assemblies[i] = Assembly.Load(AssemblyPaths[i]);
            }
            // var doMain = AppDomain.CurrentDomain;
            // // var assemblies = doMain.ReflectionOnlyGetAssemblies(); 也可以通过反射
            // var assemblies = doMain.GetAssemblies();
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
                    var attr = targetType.GetCustomAttribute<RuleCheckAttribute>();
                    if (attr == null)
                        continue;
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
            checks.Sort((x, y) => y.Setting.Priority.CompareTo(x.Setting.Priority));
        }

        public static Dictionary<string,string> ReadFixTypes()
        {
            Assembly[] assemblies = new Assembly[AssemblyPaths.Length];
            for (int i = 0; i < AssemblyPaths.Length; i++)
            {
                assemblies[i] = Assembly.Load(AssemblyPaths[i]);
            }
            Dictionary<string, string> fixers = new Dictionary<string, string>();
            string fixRule = "UChecker.Editor.IRuleFixer";
            // var doMain = AppDomain.CurrentDomain;
            // var assemblies = doMain.ReflectionOnlyGetAssemblies(); 也可以通过反射
            // var assemblies = doMain.GetAssemblies();
            // 遍历，所有的程序集
            for (int i = 0; i < assemblies.Length; i++)
            {
                //TODO 此处可以过滤掉不需要的程序集，提升效率
                var assembly = assemblies[i];
                var types = assembly.GetTypes();
                // 遍历程序集内所有的类型
                for (int j = 0; j < types.Length; j++)
                {
                    var type = types[j];
                    var typeNameSpace = types[j].Namespace;
                    // 过滤命名空间，提升效率
                    if (string.IsNullOrEmpty(typeNameSpace) || !typeNameSpace.Equals(NAME_SPACE_FIX))
                    {
                        continue;
                    }
                    Type[] typeParameters = type.GetInterfaces();
                    foreach (var t in typeParameters)
                    {
                        if (t.FullName.Contains(fixRule))
                        {
                            Type[] typeParameters2 = t.GetGenericArguments();
                            foreach (var s in typeParameters2)
                            {
                                fixers.Add(s.FullName,type.FullName);
                            }
                        }
                        Debug.Log(t.FullName);
                    }
                }
            }
            foreach (var fixer in fixers)
            {
                Debug.Log($"{fixer.Value} fix rule : {fixer.Key}");
            }
            return fixers;
        }
        public static void SaveConfig()
        {
            if (s_setting != null)
            {
                File.WriteAllText(CONFIG_PATH, Newtonsoft.Json.JsonConvert.SerializeObject(s_setting, Formatting.Indented));
            }
        }
    }
}