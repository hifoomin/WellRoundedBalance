namespace WellRoundedBalance.Items.Greens
{
    public class LeechingSeed : ItemBase<LeechingSeed>
    {
        public override string Name => ":: Items :: Greens :: Leeching Seed";
        public override ItemDef InternalPickup => RoR2Content.Items.Seed;

        public override string PickupText => "Dealing damage heals you.";

        public override string DescText => "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>1 <style=cStack>(+1 per stack)</style> health</style>, plus an additional <style=cIsHealing>" + baseHealingRegardlessOfSource + "</style> <style=cStack>(+" + healingRegardlessOfSourcePerStack + " per stack)</style> <style=cIsHealing>health</style> regardless of source.";

        [ConfigField("Base Healing Regardless of Source", "", 0.6f)]
        public static float baseHealingRegardlessOfSource;

        [ConfigField("Healing Regardless of Source Per Stack", "", 0.3f)]
        public static float healingRegardlessOfSourcePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var attackerBody = damageReport.attackerBody;
            if (damageReport != null && attackerBody != null)
            {
                var HealMask = damageReport.damageInfo.procChainMask;
                if (attackerBody.inventory)
                {
                    var stack = attackerBody.inventory.GetItemCount(RoR2Content.Items.Seed);
                    if (stack > 0)
                    {
                        attackerBody.healthComponent.Heal(baseHealingRegardlessOfSource + healingRegardlessOfSourcePerStack * (stack - 1), HealMask, true);
                    }
                }
            }
        }
    }
}