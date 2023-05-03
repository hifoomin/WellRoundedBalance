using static RoR2.CombatDirector;
using System;
using BepInEx.Configuration;

namespace WellRoundedBalance.Elites
{
    public class ChangeStats : EliteBase<ChangeStats>
    {
        public static ConfigEntry<bool> enable { get; set; }

        public override string Name => ":: Elites : Stat & Drop Rate Changes";

        [ConfigField("Tier 1 Cost Multiplier", "", 8f)]
        public static float tier1CostMultiplier;

        [ConfigField("Tier 1 Honor Cost Multiplier", "", 4.66666f)]
        public static float tier1HonorCostMultiplier;

        [ConfigField("Tier 2 Cost Multiplier", "", 48f)]
        public static float tier2CostMultiplier;

        [ConfigField("Tier 2 Health Multiplier", "", 6f)]
        public static float tier2HealthMultiplier;

        [ConfigField("Tier 2 Honor Health Multiplier", "", 4.5f)]
        public static float tier2HonorHealthMultiplier;

        [ConfigField("All Tier Damage Multiplier", "", 1f)]
        public static float allTierDamageMultiplier;

        [ConfigField("All Tier Honor Damage Multiplier", "", 1f)]
        public static float allTierHonorDamageMultiplier;

        [ConfigField("Aspect Chance", "Decimal.", 0.006f)]
        public static float aspectChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.Init += CombatDirector_Init;
        }

        private void CombatDirector_Init(On.RoR2.CombatDirector.orig_Init orig)
        {
            // Main.WRBLogger.LogError("combat director init pre orig ran");
            orig();
            foreach (EliteTierDef eliteTierDef in eliteTiers)
            {
                if (eliteTierDef != null && eliteTierDef.eliteTypes.Length > 0)
                {
                    List<EliteDef> eliteDefList = eliteTierDef.eliteTypes.ToList();

                    if (eliteDefList.Contains(RoR2Content.Elites.Fire))
                    {
                        eliteTierDef.costMultiplier = tier1CostMultiplier;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.FireHonor))
                    {
                        eliteTierDef.costMultiplier = tier1HonorCostMultiplier;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.Poison))
                    {
                        eliteTierDef.costMultiplier = tier2CostMultiplier;
                    }
                    // havent tested t2 or honor lol, but t1 elites were too frequent based on playtesting

                    foreach (EliteDef eliteDef in eliteDefList)
                    {
                        if (eliteDef != null)
                        {
                            if (eliteDef.name.IndexOf("honor", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                eliteDef.damageBoostCoefficient = allTierHonorDamageMultiplier;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, tier2HonorHealthMultiplier);
                            }
                            else
                            {
                                eliteDef.damageBoostCoefficient = allTierDamageMultiplier;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, tier2HealthMultiplier);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                var index = EliteCatalog.eliteDefs[i];
                if (index.eliteEquipmentDef)
                {
                    index.eliteEquipmentDef.dropOnDeathChance = aspectChance;
                }
            }
        }
    }
}