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
            MaxTargets = ConfigOption(10, "Max Targets", "Vanilla is 10");
            StackMaxTargets = ConfigOption(5, "Stack Max Targets", "Per Stack. Vanilla is 5");
            Range = ConfigOption(30, "Range", "Vanilla is 30");
            ProcCoefficient = ConfigOption(0.33f, "Proc Coefficient", "Vanilla is 0.33");
            Damage = ConfigOption(1f, "TOTAL Damage", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(100f),
                    x => x.MatchLdcR4(100f),
                    x => x.MatchLdcR4(20f)))
            {
                c.Index += 2;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sentient Meat Hook Chance hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(5),
                    x => x.MatchLdloc(13),
                    x => x.MatchLdcI4(5)))
            {
                c.Next.Operand = MaxTargets - StackMaxTargets;
                c.Index += 2;
                c.Next.Operand = StackMaxTargets;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sentient Meat Hook Max Targets hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(30f),
                    x => x.MatchLdloc(66),
                    x => x.MatchLdloc(62)))
            {
                c.Next.Operand = Range;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sentient Meat Hook Range hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchStloc(64)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sentient Meat Hook Damage hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(71),
                    x => x.MatchLdcR4(0.33f)))
            {
                c.Index += 1;
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sentient Meat Hook Proc Coefficient hook");
            }
        }
    }
}