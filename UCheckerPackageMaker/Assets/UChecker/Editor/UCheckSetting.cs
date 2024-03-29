﻿using System;
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
        public List<CommonCheck>Checks;
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
                setting.Checks = new List<CommonCheck>();
                AddBasicCheckSetting(setting);
                var fixers = ReadFixTypes();
                if (File.Exists(CONFIG_PATH))
                {
                    var localConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UCheckSetting>(File.ReadAllText(CONFIG_PATH));
                    MergeUCheckSetting(setting, localConfig);
                    SaveConfig();
                }
                s_setting = setting;
                MapCheckFixer(s_setting.Checks, fixers);
                SaveConfig();
            }

            return s_setting;
        }

   
        private static void MergeUCheckSetting(UCheckSetting target, UCheckSetting input)
        {
            target.GlobalDefaultPaths = input.GlobalDefaultPaths;
            target.GlobalWhiteListPaths = input.GlobalWhiteListPaths;
            Debug.Log("input from local CommonChecks: " + input.Checks.Count);
            target.Checks.RemoveAll(t => input.Checks.Exists(f =>
            {
                bool find =  t.Setting.Title.Equals(f.Setting.Title);
                if (find)
                {
                    f.Category = t.Category;
                }
                return find;
            }));
            target.Checks.AddRange(input.Checks);
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
        
        private static void AddBasicCheckSetting(UCheckSetting setting)
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
                    switch (attr.category)
                    {
                        case ERuleCategory.BasicCheck:
                        case ERuleCategory.Custom:
                        case ERuleCategory.Template:
                            var check = new CommonCheck(targetType)
                            {
                                Category = attr.category,
                                Setting =
                                {
                                    Title = attr.title,
                                    Rule = attr.rule,
                                    EnableCheck = attr.enableCheck,
                                    EnableFix = attr.enableFix,
                                    EnableCustomConfig = attr.enableCustomConfig,
                                    Priority = attr.priority
                                }
                            };
                            setting.Checks.Add(check);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
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