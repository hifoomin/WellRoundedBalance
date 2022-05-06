using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

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
            var config = Main.UCRConfig.Bind(Name, name, value, description);
            switch (value)
            {
                case bool:
                    ModSettingsManager.AddOption(new CheckBoxOption(config);
                    break;

                case float:
                    ModSettingsManager.AddOption(new StepSliderOption(config, new StepSliderConfig() { increment = value / 10f, min = 0f, max = value * 10f }));
                    break;

                case int:
                    ModSettingsManager.AddOption(new StepSliderOption(config, new StepSliderConfig() { increment = value / 10, min = 0, max = value * 10 }));
                    break;

                default:
                    break;
            }
            return config;
            //return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
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