using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using UnityEngine;

namespace UltimateCustomRun.Global
{
    public abstract class GlobalBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;
        public ConfigEntryBase config;

        public T ConfigOption<T>(T value, string name, string description)
        {
            config = Main.UCRGConfig.Bind(Name, name, value, description);
            var tabID = 0;
            var ModName = "";

            if (Name.Contains("Global"))
            {
                tabID = 7;
                ModName = "Global";
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
                        ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, increment = 0.01f, min = 0f, max = 200f }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    else
                    {
                        ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, increment = (float)config.DefaultValue / 10f, min = 0f, max = (float)config.DefaultValue * 10f }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
                    }
                    break;

                case int:
                    if ((int)config.DefaultValue == 0)
                    {
                        ModSettingsManager.AddOption(new IntSliderOption((ConfigEntry<int>)config, new IntSliderConfig() { restartRequired = true, min = 0, max = 200 }), "UltimateCustomRun.TabID." + tabID, "UCR: " + ModName);
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

            return Main.UCRGConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}