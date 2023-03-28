using MonoMod.Cil;

namespace WellRoundedBalance.Items.Yellows
{
    public class TitanicKnurl : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Titanic Knurl";
        public override ItemDef InternalPickup => RoR2Content.Items.Knurl;

        public override string PickupText => "Gain a 10% chance on hit to summon a stone fist.";

        public override string DescText => "Gain a <style=cIsDamage>" + baseFistChance + "%</style> <style=cStack>(+" + baseFistChance + "% per stack)</style> chance on hit to summon a stone fist that deals <style=cIsDamage>" + d(fistBaseDamage) + "</style> damage and <style=cIsUtility>knocks up</style> enemies in a small radius.";

        [ConfigField("Base Fist Chance", 10f)]
        public static float baseFistChance;

        [ConfigField("Fist Chance Per Stack", 10f)]
        public static float fistChancePerStack;

        [ConfigField("Fist Base Damage", "Decimal.", 3.5f)]
        public static float fistBaseDamage;

        [ConfigField("Proc Coefficient", 0f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
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
                Logger.LogError("Failed to apply Titanic Knurl Health hook");
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
                Logger.LogError("Failed to apply Titanic Knurl Regen hook");
            }
        }
    }
}