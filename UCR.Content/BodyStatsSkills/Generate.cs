using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UltimateCustomRun.BodyStatsSkills.ROO;
using System.Linq;
using RiskOfOptions;
using RiskOfOptions.Options;
using System.Text.RegularExpressions;

namespace UltimateCustomRun.BodyStatsSkills
{
    public static class Generate
    {
        public static ConfigFile UCRSSConfig;
        public static List<string> survivorNames = new();

        internal class ModConfig
        {
            public static ConfigEntry<KeyboardShortcut> reloadKeyBind;
            public static ConfigEntry<bool> generateConfigs;
            public static ConfigEntry<bool> midRunChanges;
            public static ConfigEntry<bool> goToConfigDumbWorkaround;
            public static ConfigEntry<bool> goToConfigDumbWorkaround2;

            public static void InitConfig(ConfigFile config)
            {
                reloadKeyBind = config.Bind("_General", "Reload Keybind", new KeyboardShortcut(KeyCode.F8), "Keybind to press to reload the mod's configs.");
                generateConfigs = config.Bind("_General", "Generate Configs", true, "If disabled, new configs will not be generated. Existing configs will still function normally. Can be used to speed up load times durring testing/playing.");
                midRunChanges = config.Bind("_General", "Mid Run Changes", true, "If enabled, the mod will attempt to make the changes mid run.");
                goToConfigDumbWorkaround = config.Bind("_General", "Change the config file value for stats in r2modman to enable them", true, "");
                goToConfigDumbWorkaround2 = config.Bind("_General", "Change the config file value for skills in r2modman to enable them", true, "");
            }
        }

        public static void Awake()
        {
            UCRSSConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.StatsAndSkills.cfg", true);
            Main.UCRLogger.LogInfo("==+----------------==STATS & SKILLS==----------------+==");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.96", "UCR: Survivor Stats");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
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
            if (ModConfig.generateConfigs.Value)
            {
                GenerateConfigs();
            }
            ChangeValues(BodyCatalog.allBodyPrefabBodyBodyComponents);
            if (Run.instance && ModConfig.midRunChanges.Value)
            {
                ChangeValues(CharacterMaster.instancesList.Select(x => x.GetBody()));
            }
            UCRSSConfig.Reload();
        }

        public static void GenerateConfigs()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(ModConfig.goToConfigDumbWorkaround), "UltimateCustomRun.TabID.96", "UCR: Survivor Stats");
            ModSettingsManager.AddOption(new CheckBoxOption(ModConfig.goToConfigDumbWorkaround), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
            ModSettingsManager.AddOption(new CheckBoxOption(ModConfig.goToConfigDumbWorkaround2), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
            ModSettingsManager.AddOption(new CheckBoxOption(ModConfig.goToConfigDumbWorkaround2), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
            foreach (CharacterBody character in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                string name = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(character.baseNameToken));

                if (name == "") continue;

                ConfigEntry<bool> isEnabled = UCRSSConfig.Bind(name, $"Enable {name}?", false, $"If true, {name}'s configs will be generated/values will be changed.");
                if (isEnabled.Value)
                {
                    foreach (var val in SurvivorModifyableValues)
                    {
                        if (UCRSSConfig.Keys.Any(x => x.Key == $"{name}: {val.Item1}")) continue;

                        if (SurvivorCatalog.FindSurvivorDefFromBody(character.gameObject) != null)
                        {
                            var config = UCRSSConfig.Bind(name, $"{name}: {val.Item1}", $"{val.Item3(character)}", $"Type: {val.Item2}, Vanilla is {val.Item3(character)}");
                            ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.96", "UCR: Survivor Stats");
                        }
                        else
                        {
                            var config = UCRSSConfig.Bind(name, $"{name}: {val.Item1}", $"{val.Item3(character)}", $"Type: {val.Item2}, Vanilla is {val.Item3(character)}");

                            ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.97", "UCR: Other Stats");
                        }
                    }
                    foreach (SkillDef skill in character.GetComponents<GenericSkill>().SelectMany(x => x.skillFamily.variants).Select(x => x.skillDef))
                    {
                        string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));

                        if (skName == "") continue;

                        ConfigEntry<bool> skEnabled = UCRSSConfig.Bind($"{name}_{skName}", $"Enable {name}_{skName}?", false, $"If true, {name}'s skill {skName}'s configs will be generated/values will be changed.");
                        if (skEnabled.Value)
                        {
                            foreach (var val in SkillModifyableValues)
                            {
                                if (UCRSSConfig.Keys.Any(x => x.Key == $"{name}: {skName} {val.Item1}")) continue;

                                if (SurvivorCatalog.FindSurvivorDefFromBody(character.gameObject) != null)
                                {
                                    var config = UCRSSConfig.Bind($"{name}: {skName}", $"{name}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                                    ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
                                }
                                else
                                {
                                    var config = UCRSSConfig.Bind($"{name}: {skName}", $"{name}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                                    ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.99", "UCR: Other Skilldefs");
                                }
                            }
                        }
                    }
                }

                string lunarName = "Lunar Skills";
                ConfigEntry<bool> lunarEnabled = UCRSSConfig.Bind(lunarName, $"{lunarName} Enable", false, $"If true, the lunar skill replacement's configs will be generated/changed.");
                if (lunarEnabled.Value)
                {
                    foreach (SkillDef skill in LunarSkills.Select(x => GetSkillFromToken(x)))
                    {
                        string skName = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(skill.skillNameToken));
                        if (skName == "") continue;

                        ConfigEntry<bool> skEnabled = UCRSSConfig.Bind($"{lunarName}_{skName}", $"{lunarName}_{skName} Enable", false, $"If true, {skName}'s configs will be generated/values will be changed.");
                        if (skEnabled.Value)
                        {
                            foreach (var val in SkillModifyableValues)
                            {
                                if (UCRSSConfig.Keys.Any(x => x.Key == $"{lunarName}: {skName} {val.Item1}")) continue;

                                var config = UCRSSConfig.Bind($"{lunarName}: {skName}", $"{lunarName}: {skName} {val.Item1}", $"{val.Item3(skill)}", $"{val.Item5} Type: {val.Item2}, Vanilla is {val.Item3(skill)}");
                                ModSettingsManager.AddOption(new StringInputFieldOption(config), "UltimateCustomRun.TabID.98", "UCR: Survivor Skilldefs");
                            }
                        }
                    }
                }
            }
        }

        public static void ChangeValues(IEnumerable<CharacterBody> characterBodies)
        {
            foreach (CharacterBody character in characterBodies)
            {
                string name = RemoveIllegalChars(Language.english.GetLocalizedStringByToken(character.baseNameToken));
                if (name == "") continue;

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
                        if (skName == "") continue;

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
            return Regex.Replace(input, @"[^\w]", "");
        }

        public static SkillDef GetSkillFromToken(string input)
        {
            return SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName(input));
        }
    }

    public class ReloadConfig : MonoBehaviour
    {
        private void Update()
        {
            if (Generate.ModConfig.reloadKeyBind.Value.IsDown())
            {
                Generate.ModConfig.InitConfig(Generate.UCRSSConfig);
                Generate.MakeChanges();
            }
        }
    }
}