using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using UnityEngine;

namespace WellRoundedBalance.Global
{
    public abstract class GlobalBase
    {
        public abstract string Name { get; }
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
        }
    }
}