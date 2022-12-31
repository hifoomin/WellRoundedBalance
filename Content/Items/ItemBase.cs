using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WellRoundedBalance.Items
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public ConfigEntryBase config;

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
            LanguageAPI.Add(pickupToken, PickupText);
            LanguageAPI.Add(descriptionToken, DescText);
        }
    }
}