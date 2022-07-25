using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Text.RegularExpressions;
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

        public T ConfigOption<T>(T value, string name, string description)
        {
            ConfigEntryBase config = Main.UCRConfig.Bind(Name, name, value, description);
            var tabID = 0;
            var ModName = "";

            if (Name.Contains("Whites"))
            {
                tabID = 1;
                ModName = "Whites";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Greens"))
            {
                tabID = 2;
                ModName = "Greens";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Reds"))
            {
                tabID = 3;
                ModName = "Reds";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Yellows"))
            {
                tabID = 4;
                ModName = "Yellows";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Lunars"))
            {
                tabID = 5;
                ModName = "Lunars";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Void Whites"))
            {
                tabID = 6;
                ModName = "Void Whites";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Void Greens"))
            {
                tabID = 7;
                ModName = "Void Greens";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            if (Name.Contains("Void Reds"))
            {
                tabID = 8;
                ModName = "Void Reds";
                ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
            }
            switch (value)
            {
                case bool:
                    ModSettingsManager.AddOption(new CheckBoxOption((ConfigEntry<bool>)config, new CheckBoxConfig() { restartRequired = true }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    break;

                case float:
                    if ((float)config.DefaultValue == 0)
                    {
                        ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, increment = 0.01f, min = 0f, max = 100f }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    else
                    {
                        ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, increment = (float)config.DefaultValue / 10f, min = 0f, max = (float)config.DefaultValue * 10f }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    break;

                case int:
                    if ((int)config.DefaultValue == 0)
                    {
                        ModSettingsManager.AddOption(new IntSliderOption((ConfigEntry<int>)config, new IntSliderConfig() { restartRequired = true, min = 0, max = 100 }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    else
                    {
                        ModSettingsManager.AddOption(new IntSliderOption((ConfigEntry<int>)config, new IntSliderConfig() { restartRequired = true, min = 0, max = (int)config.DefaultValue * 10 }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    break;

                default:
                    Main.UCRLogger.LogInfo("Failed to add a Risk Of Options config for" + Name);
                    break;
            }

            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public string Trim(string Text)
        {
            Text = Text.Replace("Items", string.Empty).Replace(":", string.Empty);
            try { Text = Text.Replace("Whites", string.Empty); } catch { }
            try { Text = Text.Replace("Greens", string.Empty); } catch { }
            try { Text = Text.Replace("Reds", string.Empty); } catch { }
            try { Text = Text.Replace("Yellows", string.Empty); } catch { }
            try { Text = Text.Replace("Lunars", string.Empty); } catch { }
            try { Text = Text.Replace("Void ", string.Empty); } catch { }
            Text = Text.Replace("  ", " ");
            Text = Regex.Replace(Text, @"(^[\s\0]+)|([\s\0]+$)", string.Empty);
            return Text;
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