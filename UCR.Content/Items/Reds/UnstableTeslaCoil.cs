using MonoMod.Cil;
using RoR2.Orbs;

namespace UltimateCustomRun.Items.Reds
{
    public class UnstableTeslaCoil : ItemBase
    {
        public static float Damage;
        public static float Radius;
        public static int MaxTargets;
        public static int StackMaxTargets;
        public static float Seconds;
        public static float Interval;
        public static float ProcCoefficient;

        public override string Name => ":: Items ::: Reds :: Unstable Tesla Coil";
        public override string InternalPickupToken => "shockNearby";
        public override bool NewPickup => true;
        public override string PickupText => "Shock all nearby enemies every " + Seconds + " seconds.";

        public override string DescText => "Fire out <style=cIsDamage>lightning</style> that hits <style=cIsDamage>" + MaxTargets + "</style> <style=cStack>(+" + StackMaxTargets + " per stack)</style> enemies for <style=cIsDamage>" + d(Damage) + "</style> base damage every <style=cIsDamage>" + Interval + "s</style>. The Tesla Coil switches off every <style=cIsDamage>" + Seconds + " seconds</style>.";

        public override void Init()
        {
            Damage = ConfigOption(2f, "Damage Increase", "Decimal. Vanilla is 2");
            Radius = ConfigOption(35f, "Range", "Vanilla is 35");
            MaxTargets = ConfigOption(3, "Base Max Targets", "Vanilla is 3");
            StackMaxTargets = ConfigOption(3, "Stack Max Targets", "Per Stack. Vanilla is 2");
            Seconds = ConfigOption(10f, "Seconds between Switch", "Vanilla is 10");
            Interval = ConfigOption(0.5f, "Hit Interval", "Vanilla is 0.5");
            ProcCoefficient = ConfigOption(0.3f, "Proc Coefficient", "Vanilla is 0.3");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ShockNearbyBodyBehavior_FixedUpdate;
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ChangeSeconds;
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ChangeDamage;
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ChangeProcCoefficient;
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ChangeRange;
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += ChangeTargets;
        }

        private void ShockNearbyBodyBehavior_FixedUpdate1(ILContext il)
        {
            throw new System.NotImplementedException();
        }

        private void ChangeSeconds(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)))
            {
                c.Next.Operand = Seconds;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Unstable Tesla Coil Seconds hook");
            }
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Unstable Tesla Coil Damage hook");
            }
        }

        private void ChangeProcCoefficient(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.3f)))
            {
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Unstable Tesla Coil Proc Coefficient hook");
            }
        }

        private void ChangeRange(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(35f)))
            {
                c.Next.Operand = Radius;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Unstable Tesla Coil Range hook");
            }
        }

        private void ChangeTargets(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2)))
            {
                c.Next.Operand = StackMaxTargets;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Unstable Tesla Coil Targets hook");
            }
        }

        private void ChangeBaseTargets()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Tesla)
                {
                    self.bouncesRemaining = MaxTargets;
                }
            };
        }

        private void ShockNearbyBodyBehavior_FixedUpdate(On.RoR2.Items.ShockNearbyBodyBehavior.orig_FixedUpdate orig, RoR2.Items.ShockNearbyBodyBehavior self)
        {
            self.teslaResetListInterval = Interval;
            orig(self);
        }
    }
}