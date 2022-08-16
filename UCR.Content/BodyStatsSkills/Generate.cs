using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UltimateCustomRun.BodyStatsSkills.ROO;
using System.Linq;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace UltimateCustomRun.BodyStatsSkills
{
    public static class Generate
    {
        public static ConfigFile UCRSSConfig;
        public static List<string> survivorNames = new();
        public static ConfigEntry<bool> generateEnemyConfigs;

        internal class ModConfig
        {
            public static void InitConfig(ConfigFile config)
            {
            }
        }

        public static void Awake()
        {
            UCRSSConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.StatsAndSkills.cfg", true);
            generateEnemyConfigs = UCRSSConfig.Bind("_General", "Generate config for enemy stats and skilldefs?", false);
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.96", "UCR: Survivor Stats");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
            Main.UCRLogger.LogInfo("==+----------------==STATS & SKILLS==----------------+==");
            ModConfig.InitConfig(UCRSSConfig);
            On.RoR2.RoR2Application.OnLoad += AfterLoad;
        }

        public static IEnumerator AfterLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            yield return orig(self);
            MakeChanges();
        }

        public static void MakeChanges()
        {
            UCRSSConfig.Reload();
            GenerateConfigs();
            ChangeValues(BodyCatalog.allBodyPrefabBodyBodyComponents);
            UCRSSConfig.Reload();
        }

        public static void GenerateConfigs()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(generateEnemyConfigs), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
            ModSettingsManager.AddOption(new CheckBoxOption(generateEnemyConfigs), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
            foreach (CharacterBody character in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                string name = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(character.baseNameToken));
                if (name == "") continue;

                foreach (var val in SurvivorModifyableValues)
                {
                    if (UCRSSConfig.Keys.Any(x => x.Key == $"{name}: {val.Item1}")) continue;

                    if (SurvivorCatalog.FindSurvivorDefFromBody(character.gameObject) != null)
                    {
                        var config = UCRSSConfig.Bind(name, $"{name}: {val.Item1}", $"{val.Item3(character)}", $"Type: {val.Item2}, Vanilla is {val.Item3(character)}");
                        ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.96", "UCR: Survivor Stats");
                    }
                    else if (generateEnemyConfigs.Value)
                    {
                        var config = UCRSSConfig.Bind(name, $"{name}: {val.Item1}", $"{val.Item3(character)}", $"Type: {val.Item2}, Vanilla is {val.Item3(character)}");
                        ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
                    }
                }

                foreach (SkillDef skill in character.GetComponents<GenericSkill>().SelectMany(x => x.skillFamily.variants).Select(x => x.skillDef))
                {
                    string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));
                    if (skName == "") continue;

                    foreach (var val in SkillModifyableValues)
                    {
                        if (UCRSSConfig.Keys.Any(x => x.Key == $"{name}: {skName} {val.Item1}")) continue;

                        if (SurvivorCatalog.FindSurvivorDefFromBody(character.gameObject) != null)
                        {
                            var config = UCRSSConfig.Bind($"{name}: {skName}", $"{name}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                            ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
                        }
                        else if (generateEnemyConfigs.Value)
                        {
                            var config = UCRSSConfig.Bind($"{name}: {skName}", $"{name}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                            ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
                        }
                    }
                }

                string lunarName = "Lunar Skills";
                foreach (SkillDef skill in LunarSkills.Select(x => GetSkillFromToken(x)))
                {
                    string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));
                    if (skName == "") continue;

                    foreach (var val in SkillModifyableValues)
                    {
                        if (UCRSSConfig.Keys.Any(x => x.Key == $"{lunarName}: {skName} {val.Item1}")) continue;

                        var config = UCRSSConfig.Bind($"{lunarName}: {skName}", $"{lunarName}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                        ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
                    }
                }
            }
        }

        public static void ChangeValues(IEnumerable<CharacterBody> characterBodies)
        {
            foreach (CharacterBody character in characterBodies)
            {
                string name = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(character.baseNameToken));
                if (UCRSSConfig.TryGetEntry(name, $"{name} Enable", out ConfigEntry<bool> enable) && enable.Value)
                {
                    foreach (var val in SurvivorModifyableValues)
                        if (UCRSSConfig.TryGetEntry(name, $"{name}_{val.Item1}", out ConfigEntry<string> entry) && entry.Value != $"{val.Item3(character)}")
                        {
                            val.Item4(character, entry.Value);
                        }

                    foreach (SkillDef skill in character.GetComponents<GenericSkill>().SelectMany(x => x.skillFamily.variants).Select(x => x.skillDef))
                    {
                        string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));
                        if (UCRSSConfig.TryGetEntry($"{name}_{skName}", $"{name}_{skName} Enable", out ConfigEntry<bool> skEnable) && skEnable.Value)
                        {
                            foreach (var val in SkillModifyableValues)
                            {
                                if (UCRSSConfig.TryGetEntry($"{name}_{skName}", $"{name}_{skName}_{val.Item1}", out ConfigEntry<string> entry) && entry.Value != $"{val.Item3(skill)}")
                                {
                                    val.Item4(skill, entry.Value);
                                }
                            }
                        }
                    }
                }
            }

            string lunarName = "Lunar Skills";
            if (UCRSSConfig.TryGetEntry(lunarName, $"{lunarName} Enable", out ConfigEntry<bool> lunarEnable) && lunarEnable.Value)
            {
                foreach (SkillDef skill in LunarSkills.Select(GetSkillFromToken))
                {
                    string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));
                    if (skName == "") continue;

                    if (UCRSSConfig.TryGetEntry($"{lunarName}_{skName}", $"{lunarName}_{skName} Enable", out ConfigEntry<bool> skEnable) && skEnable.Value)
                    {
                        foreach (var val in SkillModifyableValues)
                        {
                            if (UCRSSConfig.TryGetEntry($"{lunarName}_{skName}", $"{lunarName}_{skName}_{val.Item1}", out ConfigEntry<string> entry) && entry.Value != $"{val.Item3(skill)}")
                            {
                                val.Item4(skill, entry.Value);
                            }
                        }
                    }
                }
            }
        }

        public static string RemoveIllegalChars(string input)
        {
            return input.Replace("\n", "").Replace("\t", "").Replace("\\", "").Replace("\"", "").Replace("\'", "").Replace("[", "").Replace("]", "");
        }

        public static SkillDef GetSkillFromToken(string input)
        {
            return SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName(input));
        }
    }
}