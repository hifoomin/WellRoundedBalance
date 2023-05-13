namespace WellRoundedBalance.Items.Yellows
{
    public class TitanicKnurl : ItemBase<TitanicKnurl>
    {
        public override string Name => ":: Items :::: Yellows :: Titanic Knurl";
        public override ItemDef InternalPickup => RoR2Content.Items.Knurl;

        public override string PickupText => rework ? "Gain a " + baseFistChance + "% chance on hit to summon a stone fist." : "Boosts health" + (armor > 0 ? ", armor" : "") + " and regeneration.";

        public override string DescText => rework ? "Gain a <style=cIsDamage>" + baseFistChance + "%</style> <style=cStack>(+" + baseFistChance + "% per stack)</style> chance on hit to summon a stone fist that deals <style=cIsDamage>" + d(fistBaseDamage) + "</style> damage and <style=cIsUtility>knocks up</style> enemies in a small radius."
                                                  : "Increase <style=cIsHealing>maximum health</style> by <style=cIsHealing>" + health + "</style> <style=cStack>(+" + health + " per stack)</style> " + (armor > 0 ? ", <style=cIsHealing>armor</style> by <style=cIsHealing>" + armor + "</style>" : "") + " and <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+" + regen + " hp/s<style=cStack>(+" + regen + " hp/s per stack)</style>.";

        [ConfigField("Base Fist Chance", 10f)]
        public static float baseFistChance;

        [ConfigField("Fist Chance Per Stack", 10f)]
        public static float fistChancePerStack;

        [ConfigField("Fist Base Damage", "Decimal.", 3.5f)]
        public static float fistBaseDamage;

        [ConfigField("Proc Coefficient", 1f)]
        public static float procCoefficient;

        [ConfigField("Enable rework?", "Reverts to vanilla item and stacking behavior if false.", true)]
        public static bool rework;

        [ConfigField("Base Health Regeneration", "Only applies if the rework is disabled.", 2f)]
        public static float regen;

        [ConfigField("Maximum Health Gain", "Only applies if the rework is disabled.", 100f)]
        public static float health;

        [ConfigField("Armor Gain", "Only applies if the rework is disabled.", 8f)]
        public static float armor;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (rework)
            {
                IL.RoR2.CharacterBody.RecalculateStats += Changes;
                GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            }
            else
            {
                IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(40f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = health;
            }
            else
            {
                Logger.LogError("Failed to apply Titanic Knurl Health 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1.6f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = regen;
            }
            else
            {
                Logger.LogError("Failed to apply Titanic Knurl Regen 1 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
                if (stack > 0)
                {
                    args.armorAdd += armor;
                }
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var master = damageReport.attackerMaster;
            var body = damageReport.attackerBody;

            var victimBody = damageReport.victimBody;

            if (body && master)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.Knurl);
                    if (stack > 0)
                    {
                        if (Util.CheckRoll((baseFistChance + fistChancePerStack * (stack - 1)) * damageReport.damageInfo.procCoefficient, master))
                        {
                            var aimRotation = body.inputBank.GetAimRay().direction;
                            var fpi = new FireProjectileInfo
                            {
                                owner = body.gameObject,
                                damage = body.damage * fistBaseDamage,
                                position = victimBody.footPosition,
                                rotation = Util.QuaternionSafeLookRotation(aimRotation),
                                crit = body.RollCrit(),
                                projectilePrefab = Projectiles.TitanFist.prefab,
                                damageColorIndex = DamageColorIndex.Default,
                                damageTypeOverride = DamageType.Generic
                            };
                            ProjectileManager.instance.FireProjectile(fpi);
                        }
                    }
                }
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(40f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Titanic Knurl Health 2 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1.6f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Titanic Knurl Regen 2 hook");
            }
        }
    }
}