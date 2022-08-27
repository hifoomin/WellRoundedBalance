using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UltimateCustomRun.Equipment
{
    public abstract class EquipmentBase
    {
        public abstract string Name { get; }
        public abstract bool NewPickup { get; }
        public abstract bool NewDesc { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public ConfigEntryBase config;

        public T ConfigOption<T>(T value, string name, string description)
        {
            ConfigEntryBase config = Main.UCREQConfig.Bind(Name, name, value, description);
            var tabID = 0;
            var ModName = "";

            if (Name.Contains("Equipment"))
            {
                tabID = 8;
                ModName = "Equipment";
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

            return Main.UCREQConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            string pickupToken = "EQUIPMENT_" + InternalPickupToken.ToUpper() + "_PICKUP";
            string descriptionToken = "EQUIPMENT_" + InternalPickupToken.ToUpper() + "_DESC";
            if (NewPickup)
            {
                LanguageAPI.Add(pickupToken, PickupText);
            }
            if (NewDesc)
            {
                LanguageAPI.Add(descriptionToken, DescText);
            }
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}