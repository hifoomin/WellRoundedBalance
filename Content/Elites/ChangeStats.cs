using static RoR2.CombatDirector;
using System.Linq;
using System;
using WellRoundedBalance.Enemies;

namespace WellRoundedBalance.Elites
{
    public class ChangeStats : EliteBase
    {
        public static float Tier1Cost;
        public static float Tier1HonorCost;
        public static float Tier2Cost;
        public override string Name => ":: Elites : Stat Changes";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.Init += Changes;
        }

        public static void Changes(On.RoR2.CombatDirector.orig_Init orig)
        {
            orig();
            foreach (EliteTierDef eliteTierDef in eliteTiers)
            {
                if (eliteTierDef != null && eliteTierDef.eliteTypes.Length > 0)
                {
                    List<EliteDef> eliteDefList = eliteTierDef.eliteTypes.ToList();
                    /*
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
                    */
                    // costs might be changed later
                    foreach (EliteDef eliteDef in eliteDefList)
                    {
                        if (eliteDef != null)
                        {
                            if (eliteDef.name.IndexOf("honor", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var eliteString = Language.GetString(eliteDef.modifierToken);
                                // Main.WRBLogger.LogError("elitedef is + " + eliteString + " (honor)");
                                // eliteDef.healthBoostCoefficient = health;
                                // left default for now
                                eliteDef.damageBoostCoefficient = 1f;
                            }
                            else
                            {
                                var eliteString = Language.GetString(eliteDef.modifierToken);
                                // Main.WRBLogger.LogError("elitedef is + " + eliteString);
                                // eliteDef.healthBoostCoefficient = health;
                                // left default for now
                                eliteDef.damageBoostCoefficient = 1f;
                            }
                        }
                    }
                }
            }
        }
    }
}