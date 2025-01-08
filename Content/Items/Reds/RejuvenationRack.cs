namespace WellRoundedBalance.Items.Reds
{
    public class RejuvenationRack : ItemBase<RejuvenationRack>
    {
        public override string Name => ":: Items ::: Reds :: Rejuvenation Rack";
        public override ItemDef InternalPickup => RoR2Content.Items.IncreaseHealing;

        public override string PickupText => "Remove knockback. Double the strength of healing.";

        public override string DescText => "Remove <style=cIsHealing>knockback</style>. <style=cIsHealing>Heal +100%</style> <style=cStack>(+100% per stack)</style> more.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamageForce_DamageInfo_bool_bool += HealthComponent_TakeDamageForce_DamageInfo_bool_bool;
            // On.RoR2.HealthComponent.TakeDamageProcessForce_Vector3_bool_bool += HealthComponent_TakeDamageProcessForce_Vector3_bool_bool;
        }

        private void HealthComponent_TakeDamageForce_Vector3_bool_bool(On.RoR2.HealthComponent.orig_TakeDamageForce_Vector3_bool_bool orig, HealthComponent self, Vector3 force, bool alwaysApply, bool disableAirControlUntilCollision)
        {
            // J why does this exist
            // cant compare attacker to victim...
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
                    if (stack > 0)
                    {
                        return;
                    }
                }
            }
            orig(self, force, alwaysApply, disableAirControlUntilCollision);
        }

        private void HealthComponent_TakeDamageForce_DamageInfo_bool_bool(On.RoR2.HealthComponent.orig_TakeDamageForce_DamageInfo_bool_bool orig, HealthComponent self, DamageInfo damageInfo, bool alwaysApply, bool disableAirControlUntilCollision)
        {
            var body = self.body;
            if (body && body.gameObject != damageInfo.attacker)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
                    if (stack > 0)
                    {
                        return;
                    }
                }
            }
            orig(self, damageInfo, alwaysApply, disableAirControlUntilCollision);
        }
    }
}