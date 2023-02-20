using static RoR2.CombatDirector;
using System;
using BepInEx.Configuration;

namespace WellRoundedBalance.Elites
{
    public class ChangeStats : EliteBase
    {
        public static ConfigEntry<bool> enable { get; set; }

        public override string Name => ":: Elites : Stat Changes";

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
                        eliteTierDef.costMultiplier = 8f;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.FireHonor))
                    {
                        eliteTierDef.costMultiplier = 4.66666f;
                    }

                    if (eliteDefList.Contains(RoR2Content.Elites.Poison))
                    {
                        eliteTierDef.costMultiplier = 48f;
                    }
                    // havent tested t2 or honor lol, but t1 elites were too frequent based on playtesting

                    foreach (EliteDef eliteDef in eliteDefList)
                    {
                        if (eliteDef != null)
                        {
                            if (eliteDef.name.IndexOf("honor", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                eliteDef.damageBoostCoefficient = 1f;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, 4.5f);
                            }
                            else
                            {
                                eliteDef.damageBoostCoefficient = 1f;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, 6f);
                            }
                        }
                    }
                }
            }
        }
    }
}