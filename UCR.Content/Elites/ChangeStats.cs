/*
using RoR2;
using MonoMod.Cil;

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
            t1dmg = ConfigOption(6f, "Tier 1 Elite Damage Multiplier", "Vanilla is 6");
            t1hp = ConfigOption(18f, "Tier 1 Elite Health Multiplier", "Vanilla is 18");
            base.Init();
        }

        public override void Hooks()
        {
            ChangeTierOne();
            IL.RoR2.CombatDirector.Init += ChangeTierTwo;
        }
        public static void ChangeTierOne()
        {
            CombatDirector.baseEliteCostMultiplier = t1cost;
            CombatDirector.baseEliteDamageBoostCoefficient = t1dmg;
            CombatDirector.baseEliteHealthBoostCoefficient = t1hp;
            // A static readonly field cannot be assigned to (except in a static constructor or a variable initalizer)
        }
        public static void ChangeTierTwo(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.CombatDirector", "baseEliteCostMultiplier"),
                x => x.MatchLdcR4(6f),
                x => x.MatchMul(),
                x => x.MatchStfld<RoR2.CombatDirector>("costMultiplier"),
                x => x.MatchLdloc(0),
                x => x.MatchLdsfld("RoR2.CombatDirector", "baseEliteDamageBoostCoefficient"),
                x => x.MatchLdcR4(3f),
                x => x.MatchMul(),
                x => x.MatchStfld<RoR2.CombatDirector>("damageBoostCoefficient"),
                x => x.MatchLdloc(0),
                x => x.MatchLdsfld("RoR2.CombatDirector", "baseEliteHealthBoostCoefficient"),
                x => x.MatchLdcR4(4.5f)
                // holy shit this is huge
            );
            c.Index += 1;
            c.Next.Operand = t2cost / t1cost;
            c.Index += 5;
            c.Next.Operand = t2dmg / t1dmg;
            c.Index += 5;
            c.Next.Operand = t2hp / t1hp;
        }
    }
}
*/