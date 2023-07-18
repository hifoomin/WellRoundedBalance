namespace WellRoundedBalance.Items.Greens
{
    public class LeechingSeed : ItemBase<LeechingSeed>
    {
        public override string Name => ":: Items :: Greens :: Leeching Seed";
        public override ItemDef InternalPickup => RoR2Content.Items.Seed;

        public override string PickupText => "Dealing damage heals you.";

        public override string DescText => "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>1 <style=cStack>(+1 per stack)</style> health</style>, plus an additional <style=cIsHealing>" + baseHealingRegardlessOfSource + "</style> <style=cStack>(+" + healingRegardlessOfSourcePerStack + " per stack)</style> <style=cIsHealing>health</style> regardless of source.";

        [ConfigField("Base Healing Regardless of Source", "", 0.5f)]
        public static float baseHealingRegardlessOfSource;

        [ConfigField("Healing Regardless of Source Per Stack", "", 0.5f)]
        public static float healingRegardlessOfSourcePerStack;

        [ConfigField("Healing Regardless of Source Cooldown", "I didn't really wanna do this but there were some survivors/reworks that made this item absolutely insane.", 0.15f)]
        public static float cooldown;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            var inventory = body.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.Seed);
                if (stack > 0 && body.GetComponent<LeechingSeedCooldown>() == null)
                {
                    body.gameObject.AddComponent<LeechingSeedCooldown>();
                }
            }
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
                    var leechSeedComp = attackerBody.GetComponent<LeechingSeedCooldown>();
                    if (stack > 0 && leechSeedComp && leechSeedComp.canHeal)
                    {
                        leechSeedComp.canHeal = false;
                        attackerBody.healthComponent.Heal(baseHealingRegardlessOfSource + healingRegardlessOfSourcePerStack * (stack - 1), HealMask, true);
                    }
                }
            }
        }
    }

    public class LeechingSeedCooldown : MonoBehaviour
    {
        public bool canHeal = true;
        public float timer;
        public float cooldown = LeechingSeed.cooldown;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (canHeal == false && timer >= cooldown)
            {
                cooldown = 0f;
                canHeal = true;
            }
        }
    }
}