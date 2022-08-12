/*
using RoR2;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static RoR2.CombatDirector;

namespace UltimateCustomRun.Elites
{
    public class ChangeStats : EnemyBase
    {
        public static float t1cost;
        public static float t1dmg;
        public static float t1hp;
        public static float t2cost;
        public static float t2dmg;
        public static float t2hp;
        public override string Name => "::::: Elites :: Tier 1 and Tier 2";

        public override void Init()
        {
            t1cost = ConfigOption(6f, "Tier 1 Elite Cost Multiplier", "Vanilla is 6");
            t1dmg = ConfigOption(2f, "Tier 1 Elite Damage Multiplier", "Vanilla is 2");
            t1hp = ConfigOption(4f, "Tier 1 Elite Health Multiplier", "Vanilla is 4");
            t2cost = ConfigOption(36f, "Tier 2 Elite Cost Multiplier", "Vanilla is 36");
            t2dmg = ConfigOption(6f, "Tier 2 Elite Damage Multiplier", "Vanilla is 6");
            t2hp = ConfigOption(18f, "Tier 2 Elite Health Multiplier", "Vanilla is 18");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.Init += Debug;
        }

        public static void Debug(On.RoR2.CombatDirector.orig_Init orig)
        {
            orig();
            for (int i = 0; i < CombatDirector.eliteTiers.Length; i++)
            {
                EliteTierDef etd = CombatDirector.eliteTiers[i];
                if (etd != null)
                {
                    Main.UCRLogger.LogInfo("[" + i + "] Tier Cost: " + etd.costMultiplier);
                    foreach (EliteDef ed in etd.eliteTypes)
                    {
                        if (ed != null)
                        {
                            Main.UCRLogger.LogInfo(ed.eliteEquipmentDef.name + " - " + ed.healthBoostCoefficient + "x health, " + ed.damageBoostCoefficient + "x damage");
                        }
                    }
                    Main.UCRLogger.LogInfo("\n");
                }
            }
        }
    }
}
*/