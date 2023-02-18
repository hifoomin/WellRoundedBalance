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
            EliteAPI.
            On.RoR2.CombatDirector.Init += CombatDirector_Init;
        }

        private void CombatDirector_Init(On.RoR2.CombatDirector.orig_Init orig)
        {
            Main.WRBLogger.LogError("combat director init pre orig ran");
            orig();
            Main.WRBLogger.LogError("combat director init post orig ran");
            for (int i = 0; i < eliteTiers.Length; i++)
            {
                Main.WRBLogger.LogError("iterating through every elite tier");
                var eliteTierDef = eliteTiers[i];
                if (eliteTierDef != null)
                {
                    Main.WRBLogger.LogError("iterating through every every elite tier def in elite tier array");
                    for (int j = 0; j < eliteTierDef.eliteTypes.Length; j++)
                    {
                        var eliteDef = eliteTierDef.eliteTypes[i];
                        Main.WRBLogger.LogFatal("iterating through every elite def in elite def array");
                        if (eliteDef != null)
                        {
                            if (eliteDef.name.IndexOf("honor", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Main.WRBLogger.LogFatal("found honor elite");
                                eliteDef.damageBoostCoefficient = 1f;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, 4f);
                                Main.WRBLogger.LogFatal("HONOR elitedef damage boost is " + eliteDef.damageBoostCoefficient);
                                Main.WRBLogger.LogFatal("HONOR elitedef HEALTH boost is " + eliteDef.healthBoostCoefficient);
                            }
                            else
                            {
                                Main.WRBLogger.LogFatal("found STANDARD elite");
                                eliteDef.damageBoostCoefficient = 1f;
                                eliteDef.healthBoostCoefficient = Mathf.Min(eliteDef.healthBoostCoefficient, 4f);
                                Main.WRBLogger.LogFatal("STANDARD elitedef damage boost is " + eliteDef.damageBoostCoefficient);
                                Main.WRBLogger.LogFatal("STANDARD elitedef HEALTH boost is " + eliteDef.healthBoostCoefficient);
                            }
                        }
                    }
                }
            }
        }
    }
}