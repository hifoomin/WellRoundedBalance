using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Equipment
{
    public class GooboJr : EquipmentBase
    {
        public override string Name => "::: Equipment :: Goobo Jr";
        public override string InternalPickupToken => "gummyClone";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Create a clone of you for 30 seconds.";

        public override string DescText => "Spawn a gummy clone that has <style=cIsDamage>" + /* + d(1f + Damage) + */"300% damage</style> and <style=cIsHealing>"/* + d(1f + Health)*/ + "300% health</style>. Expires in <style=cIsUtility>" + Duration + "</style> seconds. Can have up to <style=cIsUtility>" + MaxGoobos + "</style> gummy clones.";

        public static float Duration;
        public static int MaxGoobos;
        // public static int Damage;
        // public static int Health;

        public override void Init()
        {
            Duration = ConfigOption(30f, "Lifetime", "Vanilla is 30");
            MaxGoobos = ConfigOption(3, "Max Goboos", "Vanilla is 3");
            // Damage = ConfigOption(2, "Damage Bonus", "Decimal. Vanilla is 2");
            // Health = ConfigOption(2, "Health Bonus", "Decimal. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Projectile.GummyCloneProjectile.OnProjectileImpact += Changes;
            IL.RoR2.CharacterMaster.SetUpGummyClone += ChangeLifetime;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
        }

        private int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.GummyClone)
            {
                return MaxGoobos;
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void ChangeLifetime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Goobo Jr. Lifetime hook");
            }
        }

        private void Changes(On.RoR2.Projectile.GummyCloneProjectile.orig_OnProjectileImpact orig, RoR2.Projectile.GummyCloneProjectile self, RoR2.Projectile.ProjectileImpactInfo impactInfo)
        {
            self.maxLifetime = Duration;
            // self.damageBoostCount = Damage;
            // self.hpBoostCount = Health;
            // does nothing
            orig(self, impactInfo);
        }

        // goobo is dumb and none of this works
    }
}