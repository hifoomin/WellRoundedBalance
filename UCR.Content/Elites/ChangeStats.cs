using RoR2;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static RoR2.CombatDirector;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UltimateCustomRun.Elites
{
    public class ChangeStats : EnemyBase
    {
        public static float Tier1Cost;
        public static float Tier1HonorCost;
        public static float Tier2Cost;
        public override string Name => "Elites :: Tier 1 and Tier 2";

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
                                float health = Main.UCREConfig.Bind("Elites : " + eliteString + " (Honor)", "Health Multiplier", eliteDef.healthBoostCoefficient, "Decimal. Vanilla is " + eliteDef.healthBoostCoefficient).Value;
                                float damage = Main.UCREConfig.Bind("Elites : " + eliteString + " (Honor)", "Damage Multiplier", eliteDef.damageBoostCoefficient, "Decimal. Vanilla is " + eliteDef.damageBoostCoefficient).Value;
                                eliteDef.healthBoostCoefficient = health;
                                eliteDef.damageBoostCoefficient = damage;
                            }
                            else
                            {
                                var eliteString = Language.GetString(eliteDef.modifierToken);
                                // Main.UCRLogger.LogError("elitedef is + " + eliteString);
                                float health = Main.UCREConfig.Bind<float>("Elites : " + eliteString, "Health Multiplier", eliteDef.healthBoostCoefficient, "Decimal. Vanilla is " + eliteDef.healthBoostCoefficient).Value;
                                float damage = Main.UCREConfig.Bind<float>("Elites : " + eliteString, "Damage Multiplier", eliteDef.damageBoostCoefficient, "Decimal. Vanilla is " + eliteDef.damageBoostCoefficient).Value;
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