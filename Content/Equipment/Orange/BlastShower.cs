using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class BlastShower : EquipmentBase<BlastShower>
    {
        public override string Name => ":: Equipment :: Blast Shower";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Cleanse;

        public override string PickupText => (projSpeedDecrease > 0 ? "Passively slow down enemy projectiles. " : "") + "Cleanse all negative effects.";

        public override string DescText => (projSpeedDecrease > 0 ? "Passively <style=cIsUtility>slow down</style> enemy projectiles by <style=cIsUtility>" + d(projSpeedDecrease) + "</style>. " : "") + "<style=cIsUtility>Cleanse</style> all negative effects. Includes debuffs, damage over time, and nearby projectiles.";

        [ConfigField("Cooldown", "", 13f)]
        public static float cooldown;

        [ConfigField("Projectile Removal Range", "", 20f)]
        public static float projectileRemovalRange;

        // [ConfigField("Passive Enemy Projectile Speed Decrease", "Decimal", 0.08f)]
        public static float projSpeedDecrease = 0;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Util.CleanseBody += ChangeRemovalRange;

            var BlastShower = Utils.Paths.EquipmentDef.Cleanse.Load<EquipmentDef>();
            BlastShower.cooldown = cooldown;
            // On.RoR2.Projectile.ProjectileController.Start += ProjectileController_Start;
            // On.RoR2.Projectile.ProjectileSimple.Start += ProjectileSimple_Start;
            // CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        // this code is awful help
        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            for (int i = 0; i < CharacterBody.readOnlyInstancesList.Count; i++)
            {
                var index = CharacterBody.readOnlyInstancesList[i];
                if (index.teamComponent.teamIndex == TeamIndex.Player)
                {
                    var inventory = index.inventory;
                    if (inventory && inventory.currentEquipmentIndex == RoR2Content.Equipment.Gateway.equipmentIndex)
                    {
                        decrease += projSpeedDecrease;
                        Main.WRBLogger.LogError("blast shower decrease is " + decrease);
                    }
                }
            }
        }

        public static float decrease = 0;

        private void ProjectileSimple_Start(On.RoR2.Projectile.ProjectileSimple.orig_Start orig, ProjectileSimple self)
        {
            if (self.rigidbody && !self.rigidbody.useGravity && self.gameObject.GetComponent<TeamFilter>().teamIndex != TeamIndex.Player)
            {
                self.desiredForwardSpeed *= 1f - decrease;
            }
            orig(self);
        }

        private void ProjectileController_Start(On.RoR2.Projectile.ProjectileController.orig_Start orig, ProjectileController self)
        {
            orig(self);
            if (self.gameObject != null && self.teamFilter.teamIndex != TeamIndex.Player && self.gameObject.GetComponent<ProjectileCharacterController>() != null)
            {
                var pcc = self.gameObject.GetComponent<ProjectileCharacterController>();
                pcc.velocity *= 1f - decrease;
            }
        }

        private void ChangeRemovalRange(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(6f)))
            {
                c.Next.Operand = projectileRemovalRange;
            }
            else
            {
                Logger.LogError("Failed to apply Blast Shower Projectile Removal hook");
            }
        }
    }
}