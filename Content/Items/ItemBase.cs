using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using BepInEx.Logging;

namespace WellRoundedBalance.Items
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public ManualLogSource Logger => Main.WRBLogger;

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public T ConfigOption<T>(T value, string name, string desc)
        {
            ConfigEntry<T> entry = Main.WRBConfig.Bind<T>(Name, name, value, desc);
            if (typeof(T) == typeof(int))
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as BepInEx.Configuration.ConfigEntry<int>));
            }
            else if (typeof(T) == typeof(float))
            {
                ModSettingsManager.AddOption(new SliderOption(entry as BepInEx.Configuration.ConfigEntry<float>));
            }
            else if (typeof(T) == typeof(string))
            {
                ModSettingsManager.AddOption(new StringInputFieldOption(entry as BepInEx.Configuration.ConfigEntry<string>));
            }
            else if (typeof(T) == typeof(Enum))
            {
                ModSettingsManager.AddOption(new ChoiceOption(entry));
            }
            return entry.Value;
        }

        public virtual void Init()
        {
            Hooks();
            string pickupToken = "ITEM_" + InternalPickupToken.ToUpper() + "_PICKUP";
            string descriptionToken = "ITEM_" + InternalPickupToken.ToUpper() + "_DESC";
            LanguageAPI.Add(pickupToken, PickupText);
            LanguageAPI.Add(descriptionToken, DescText);
        }

        public string GetToken(string addressablePath)
        {
            ItemDef def = Addressables.LoadAssetAsync<ItemDef>(addressablePath).WaitForCompletion();
            string token = def.nameToken;
            token = token.Replace("ITEM_", "");
            token = token.Replace("_NAME", "");
            return token;
        }
    }
}