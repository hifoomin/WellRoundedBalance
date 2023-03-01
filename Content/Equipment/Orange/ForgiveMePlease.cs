using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class ForgiveMePlease : EquipmentBase
    {
        public override string Name => "::: Equipment :: Forgive Me Please";
        public override string InternalPickupToken => "deathProjectile";

        public override string PickupText => "Throw a cursed doll that repeatedly triggers your 'On Kill' effects.";

        public override string DescText => "Throw a cursed doll out that <style=cIsDamage>triggers</style> any <style=cIsDamage>On-Kill</style> effects you have every <style=cIsUtility>" + interval + "</style> second for <style=cIsUtility>" + maxDuration + "</style> seconds.";

        [ConfigField("Interval", "Formula for activation count: Max Duration / Interval", 1f)]
        public static float interval;

        [ConfigField("Max Duration", "Formula for activation count: Max Duration / Interval", 8f)]
        public static float maxDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Projectile.DeathProjectile.FixedUpdate += ChangeInterval;
            On.RoR2.Projectile.DeathProjectile.Awake += ChangeMaxDur;
        }

        private void ChangeMaxDur(On.RoR2.Projectile.DeathProjectile.orig_Awake orig, DeathProjectile self)
        {
            self.baseDuration = maxDuration;
            orig(self);
        }

        private void ChangeInterval(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(DeathProjectile), (nameof(DeathProjectile.fixedAge))),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = interval;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Forgive Me Please Interval hook");
            }
        }
    }
}