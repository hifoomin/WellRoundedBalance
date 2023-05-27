namespace WellRoundedBalance.Items.Whites
{
    public class ArmorPiercingRounds : ItemBase<ArmorPiercingRounds>
    {
        public override string Name => ":: Items : Whites :: Armor Piercing Rounds";
        public override ItemDef InternalPickup => RoR2Content.Items.BossDamageBonus;

        public override string PickupText => "Deal extra damage to bosses" + (championDamageBonus > 0 ? " and champions." : ".");

        public override string DescText => "Deal an additional <style=cIsDamage>20%</style> damage <style=cStack>(+20% per stack)</style> to bosses" + (championDamageBonus > 0 ? " and <style=cIsDamage>" + d(championDamageBonus) + "</style> damage <style=cStack>(+" + d(championDamageBonus) + " per stack)</style> to champions." : ".");

        [ConfigField("Champion Damage Bonus", "Decimal.", 0.1f)]
        public static float championDamageBonus;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent victim, DamageInfo damageInfo)
        {
            var victimBody = victim.body;
            if (victimBody)
            {
                var attacker = damageInfo.attacker;
                if (attacker)
                {
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        var inventory = attackerBody.inventory;
                        if (inventory)
                        {
                            var stack = inventory.GetItemCount(RoR2Content.Items.BossDamageBonus);
                            if (victimBody.isChampion && !victimBody.isBoss && stack > 0) // not boss to prevent double dipping
                            {
                                damageInfo.damage *= 1f + championDamageBonus * stack;
                                damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                                EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.bossDamageBonusImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
                            }
                        }
                    }
                }
            }

            orig(victim, damageInfo);
        }
    }
}