using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UltimateCustomRun
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public abstract bool NewPickup { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public ConfigEntryBase config;
        public AssetBundle UCRCopy;

        public T ConfigOption<T>(T value, string name, string description)
        {
            config = Main.UCRConfig.Bind(Name, name, value, description);
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public string Trim(string Text)
        {
            Text = Text.Replace("Items", "").Replace(":", "");
            try { Text = Text.Replace("Whites", ""); } catch { }
            try { Text = Text.Replace("Greens", ""); } catch { }
            try { Text = Text.Replace("Reds", ""); } catch { }
            try { Text = Text.Replace("Yellows", ""); } catch { }
            try { Text = Text.Replace("Lunars", ""); } catch { }
            Text = Text.Replace("  ", " ");
            return Text.Trim();
        }

        public void ROSOption(string ModName, float Minimum, float Maximum, float Incr, string tabID)
        {
            var field = typeof(ConfigDefinition).GetField("<Section>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(config.Definition, Trim(config.Definition.Section));
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR:" + ModName);

            switch (config.DefaultValue)
            {
                // shouldve done switch for ModName and changed tabID based on that
                case bool:
                    ModSettingsManager.AddOption(new CheckBoxOption((ConfigEntry<bool>)config, new CheckBoxConfig() { restartRequired = true }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    Main.UCRLogger.LogError("Added" + Trim(config.Definition.Section));
                    // shrug
                    break;

                case float:
                    ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, increment = Incr, min = Minimum, max = Maximum }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    Main.UCRLogger.LogError("Added" + Trim(config.Definition.Section));
                    break;

                case int:
                    ModSettingsManager.AddOption(new IntSliderOption((ConfigEntry<int>)config, new IntSliderConfig() { restartRequired = true, min = (int)Minimum, max = (int)Maximum }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    Main.UCRLogger.LogError("Added" + Trim(config.Definition.Section));
                    break;

                default:
                    Main.UCRLogger.LogInfo("Failed to add a Risk Of Options config for" + Name);
                    break;
            }
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            string pickupToken = "ITEM_" + InternalPickupToken.ToUpper() + "_PICKUP";
            string descriptionToken = "ITEM_" + InternalPickupToken.ToUpper() + "_DESC";
            if (NewPickup)
            {
                LanguageAPI.Add(pickupToken, PickupText);
            }
            LanguageAPI.Add(descriptionToken, DescText);
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}