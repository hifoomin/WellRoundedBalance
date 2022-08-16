using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class ForgiveMePlease : EquipmentBase
    {
        public override string Name => "::: Equipment :: Forgive Me Please";
        public override string InternalPickupToken => "deathProjectile";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Throw a cursed doll out that <style=cIsDamage>triggers</style> any <style=cIsDamage>On-Kill</style> effects you have every <style=cIsUtility>" + Interval + "</style> second for <style=cIsUtility>" + MaxDuration + "</style> seconds.";

        public static float Interval;
        public static float MaxDuration;

        public override void Init()
        {
            Interval = ConfigOption(1f, "Interval", "Vanilla is 1");
            MaxDuration = ConfigOption(8f, "Max Duration", "Vanilla is 8\nFormula: Max Duration / Interval = Activation Count");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Projectile.DeathProjectile.FixedUpdate += ChangeInterval;
            On.RoR2.Projectile.DeathProjectile.Awake += ChangeMaxDur;
        }

        private void ChangeMaxDur(On.RoR2.Projectile.DeathProjectile.orig_Awake orig, RoR2.Projectile.DeathProjectile self)
        {
            self.baseDuration = MaxDuration;
            orig(self);
        }

        private void ChangeInterval(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(RoR2.Projectile.DeathProjectile), (nameof(RoR2.Projectile.DeathProjectile.fixedAge))),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = Interval;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Forgive Me Please Interval hook");
            }
        }
    }
}