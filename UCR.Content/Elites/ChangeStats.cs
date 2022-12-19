using RoR2;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static RoR2.CombatDirector;
using System.Collections.Generic;
using System.Linq;
using System;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using BepInEx.Configuration;
using UnityEngine;

namespace UltimateCustomRun.Elites
{
    public class ChangeStats : EnemyBase
    {
        public static float Tier1Cost;
        public static float Tier1HonorCost;
        public static float Tier2Cost;
        public override string Name => "Elites";

        public override void Init()
        {
            Tier1Cost = ConfigOption(6f, "Tier 1 Elite Cost Multiplier", "Vanilla is 6");
            Tier1HonorCost = ConfigOption(3.5f, "Tier 1 Honor Elite Cost Multiplier", "Vanilla is 3.5");
            Tier2Cost = ConfigOption(36f, "Tier 2 Elite Cost Multiplier", "Vanilla is 36");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.Init += Changes;
            ModSettingsManager.SetModIcon(Main.UCR.LoadAsset<Sprite>("texUCRIcon.png"), "UltimateCustomRun.TabID.69", "UCR: Elites");
        }

        public static void Changes(On.RoR2.CombatDirector.orig_Init orig)
        {
            orig();
            foreach (EliteTierDef eliteTierDef in CombatDirector.eliteTiers)
            {
                if (eliteTierDef != null && eliteTierDef.eliteTypes.Length > 0)
                {
                    List<EliteDef> eliteDefList = eliteTierDef.eliteTypes.ToList();

                    if (eliteDefList.Contains(RoR2Content.Elites.Fire))
                    {
                        eliteTierDef.costMultiplier = Tier1Cost;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.FireHonor))
                    {
                        eliteTierDef.costMultiplier = Tier1HonorCost;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.Poison))
                    {
                        eliteTierDef.costMultiplier = Tier2Cost;
                    }

                    foreach (EliteDef eliteDef in eliteDefList)
                    {
                        if (eliteDef != null)
                        {
                            if (eliteDef.name.IndexOf("honor", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var eliteString = Language.GetString(eliteDef.modifierToken);
                                // Main.UCRLogger.LogError("elitedef is + " + eliteString + " (honor)");
                                ConfigEntry<float> healthConfig = Main.UCREConfig.Bind("Elites : " + eliteString + " (Honor)", "Health Multiplier", eliteDef.healthBoostCoefficient, "Decimal. Vanilla is " + eliteDef.healthBoostCoefficient);
                                ConfigEntry<float> damageConfig = Main.UCREConfig.Bind("Elites : " + eliteString + " (Honor)", "Damage Multiplier", eliteDef.damageBoostCoefficient, "Decimal. Vanilla is " + eliteDef.damageBoostCoefficient);
                                float health = healthConfig.Value;
                                var damage = damageConfig.Value;
                                ModSettingsManager.AddOption(new StepSliderOption(healthConfig, new StepSliderConfig() { restartRequired = true, increment = (float)healthConfig.DefaultValue / 10f, min = 0f, max = (float)healthConfig.DefaultValue * 10f }), "UltimateCustomRun.TabID.69", "UCR: Elites");
                                ModSettingsManager.AddOption(new StepSliderOption(damageConfig, new StepSliderConfig() { restartRequired = true, increment = (float)damageConfig.DefaultValue / 10f, min = 0f, max = (float)damageConfig.DefaultValue * 10f }), "UltimateCustomRun.TabID.69", "UCR: Elites");
                                eliteDef.healthBoostCoefficient = health;
                                eliteDef.damageBoostCoefficient = damage;
                            }
                            else
                            {
                                var eliteString = Language.GetString(eliteDef.modifierToken);
                                // Main.UCRLogger.LogError("elitedef is + " + eliteString);
                                ConfigEntry<float> healthConfig = Main.UCREConfig.Bind<float>("Elites : " + eliteString, "Health Multiplier", eliteDef.healthBoostCoefficient, "Decimal. Vanilla is " + eliteDef.healthBoostCoefficient);
                                ConfigEntry<float> damageConfig = Main.UCREConfig.Bind<float>("Elites : " + eliteString, "Damage Multiplier", eliteDef.damageBoostCoefficient, "Decimal. Vanilla is " + eliteDef.damageBoostCoefficient);
                                float health = healthConfig.Value;
                                var damage = damageConfig.Value;
                                ModSettingsManager.AddOption(new StepSliderOption(healthConfig, new StepSliderConfig() { restartRequired = true, increment = (float)healthConfig.DefaultValue / 10f, min = 0f, max = (float)healthConfig.DefaultValue * 10f }), "UltimateCustomRun.TabID.69", "UCR: Elites");
                                ModSettingsManager.AddOption(new StepSliderOption(damageConfig, new StepSliderConfig() { restartRequired = true, increment = (float)damageConfig.DefaultValue / 10f, min = 0f, max = (float)damageConfig.DefaultValue * 10f }), "UltimateCustomRun.TabID.69", "UCR: Elites");
                                eliteDef.healthBoostCoefficient = health;
                                eliteDef.damageBoostCoefficient = damage;
                            }
                        }
                    }
                }
            }
        }
    }
}