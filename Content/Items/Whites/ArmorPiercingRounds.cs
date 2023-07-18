using System;

namespace WellRoundedBalance.Items.Whites
{
    public class ArmorPiercingRounds : ItemBase<ArmorPiercingRounds>
    {
        public override string Name => ":: Items : Whites :: Armor Piercing Rounds";
        public override ItemDef InternalPickup => RoR2Content.Items.BossDamageBonus;

        public override string PickupText => "Deal extra damage to bosses and champions.";

        public override string DescText => "Deal an additional <style=cIsDamage>" + d(bossChampionDamageBonus) + "</style> <style=cStack>(+" + d(bossChampionDamageBonus) + " per stack)</style> damage to bosses and champions.";

        [ConfigField("Boss and Champion Damage Bonus", "Decimal.", 0.15f)]
        public static float bossChampionDamageBonus;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage1;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "BossDamageBonus"),
                x => x.MatchCallOrCallvirt(typeof(Inventory).GetMethod("GetItemCount", new Type[] { typeof(ItemDef) })),
                x => x.MatchStloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(0)))
            {
                c.Index += 5;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return int.MaxValue;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Armor Piercing Rounds Deletion hook");
            }
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
                            if ((victimBody.isChampion || victimBody.isBoss) && stack > 0)
                            {
                                damageInfo.damage *= 1f + bossChampionDamageBonus * stack;
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