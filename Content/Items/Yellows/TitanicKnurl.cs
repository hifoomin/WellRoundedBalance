using MonoMod.Cil;
using R2API.Utils;
using UnityEngine.UIElements;

namespace WellRoundedBalance.Items.Yellows
{
    public class TitanicKnurl : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Titanic Knurl";
        public override string InternalPickupToken => "knurl";

        public override string PickupText => "Gain a 10% chance on hit to summon a stone fist.";

        public override string DescText => "Gain a <style=cIsDamage>10%</style> <style=cStack>(+10% per stack)</style> chance on hit to summon a stone fist that deals <style=cIsDamage>300%</style> damage and <style=cIsUtility>knocks up</style> enemies in a small radius.";

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
                        if (Util.CheckRoll(10f * damageReport.damageInfo.procCoefficient * stack, master))
                        {
                            var aimRotation = body.inputBank.GetAimRay().direction;
                            var fpi = new FireProjectileInfo
                            {
                                owner = body.gameObject,
                                damage = body.damage * 3f,
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
                Main.WRBLogger.LogError("Failed to apply Titanic Knurl Health hook");
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
                Main.WRBLogger.LogError("Failed to apply Titanic Knurl Regen hook");
            }
        }
    }
}