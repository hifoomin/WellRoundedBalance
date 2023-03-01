namespace WellRoundedBalance.Equipment
{
    public class GooboJr : EquipmentBase
    {
        public override string Name => ":: Equipment :: Goobo Jr";

        public override string InternalPickupToken => "gummyClone";

        public override string PickupText => "Create a clone of you for " + maxLifetime + " seconds.";

        public override string DescText => "Spawn a gummy clone that has <style=cIsDamage>300% damage</style> and <style=cIsHealing>300% health</style>. Expires in <style=cIsUtility>" + maxLifetime + "</style> seconds. Can have up to <style=cIsUtility>" + maxGoobos + " Goobers</style>.";

        [ConfigField("Cooldown", "", 60f)]
        public static float cooldown;

        [ConfigField("Max Lifetime", "", 30f)]
        public static float maxLifetime;

        [ConfigField("Max Goobos", "", 3)]
        public static int maxGoobos;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var Goobo = Utils.Paths.EquipmentDef.GummyClone.Load<EquipmentDef>();
            Goobo.cooldown = cooldown;

            On.RoR2.Projectile.GummyCloneProjectile.OnProjectileImpact += GummyCloneProjectile_OnProjectileImpact;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.GummyClone)
            {
                return maxGoobos;
            }
            return orig(self, slot);
        }

        private void GummyCloneProjectile_OnProjectileImpact(On.RoR2.Projectile.GummyCloneProjectile.orig_OnProjectileImpact orig, GummyCloneProjectile self, ProjectileImpactInfo impactInfo)
        {
            self.maxLifetime = maxLifetime;
            orig(self, impactInfo);
        }
    }
}