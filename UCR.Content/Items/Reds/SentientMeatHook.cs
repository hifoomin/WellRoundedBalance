using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class SentientMeatHook : ItemBase
    {
        public static float Chance;
        public static int MaxTargets;
        public static int StackMaxTargets;
        public static float Range;
        public static float ProcCoefficient;
        public static float Damage;

        public override string Name => ":: Items ::: Reds :: Sentient Meat Hook";
        public override string InternalPickupToken => "bounceNearby";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> chance on hit to <style=cIsDamage>fire homing hooks</style> at up to <style=cIsDamage>" + MaxTargets + "</style> <style=cStack>(+" + StackMaxTargets + " per stack)</style> enemies for <style=cIsDamage>100%</style> TOTAL damage.";

        public override void Init()
        {
            Chance = ConfigOption(20f, "Chance", "Per Stack. Vanilla is 20");
            ROSOption("Greens", 0f, 100f, 1f, "3");
            MaxTargets = ConfigOption(10, "Max Targets", "Vanilla is 10");
            ROSOption("Greens", 0f, 30f, 1f, "3");
            StackMaxTargets = ConfigOption(5, "Stack Max Targets", "Per Stack. Vanilla is 5");
            ROSOption("Greens", 0f, 30f, 1f, "3");
            Range = ConfigOption(30, "Range", "Vanilla is 30");
            ROSOption("Greens", 0f, 100f, 1f, "3");
            ProcCoefficient = ConfigOption(0.33f, "Proc Coefficient", "Vanilla is 0.33");
            ROSOption("Greens", 0f, 1f, 0.033f, "3");
            Damage = ConfigOption(1f, "TOTAL Damage", "Decimal. Vanilla is 1");
            ROSOption("Greens", 0f, 10f, 0.05f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(100f),
                x => x.MatchLdcR4(100f),
                x => x.MatchLdcR4(20f)
            );
            c.Index += 2;
            c.Next.Operand = Chance;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(5),
                x => x.MatchLdloc(13),
                x => x.MatchLdcI4(5)
            );
            c.Next.Operand = MaxTargets - StackMaxTargets;
            c.Index += 2;
            c.Next.Operand = StackMaxTargets;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f),
                x => x.MatchLdloc(66),
                x => x.MatchLdloc(62)
            );
            c.Next.Operand = Range;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchStloc(64)
            );
            c.Next.Operand = Damage;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(71),
                x => x.MatchLdcR4(0.33f)
            );
            c.Index += 1;
            c.Next.Operand = ProcCoefficient;
        }
    }
}