using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System;

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

        public T ConfigOption<T>(T value, string name, string description)
        {
            ConfigEntryBase config = Main.UCRConfig.Bind(Name, name, value, description);
            var cachedDefaultValue = config.DefaultValue;
            switch (value)
            {
                case bool:
                    ModSettingsManager.AddOption(new CheckBoxOption((ConfigEntry<bool>)config, new CheckBoxConfig() { restartRequired = true }));
                    break;

                case float:
                    ModSettingsManager.AddOption(new StepSliderOption((ConfigEntry<float>)config, new StepSliderConfig() { restartRequired = true, /*increment = (float)cachedDefaultValue / 10f,*/ increment = 0.000001f, min = 0f/*, max = (float)cachedDefaultValue * 10f*/ }));
                    Main.UCRLogger.LogWarning(Name + " - the slider comes from type" + config + ". The specific ConfigOption is " + name + ". The ConfigOption, when casted to (float)config.DefaultValue is " + (float)cachedDefaultValue);
                    break;

                case int:
                    ModSettingsManager.AddOption(new IntSliderOption((ConfigEntry<int>)config, new IntSliderConfig() { restartRequired = true, min = 0/*, max = (int)cachedDefaultValue * 10*/ }));
                    Main.UCRLogger.LogWarning(Name + " - the slider comes from type" + config + ". The specific ConfigOption is " + name + ". The ConfigOption, when casted to (int)config.DefaultValue is " + (int)cachedDefaultValue);
                    break;

                default:
                    Main.UCRLogger.LogInfo("Failed to add a Risk Of Options config for" + Name);
                    break;
            }

            //return config;
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
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