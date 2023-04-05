using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class RunaldsBand : ItemBase<RunaldsBand>
    {
        public override string Name => ":: Items :: Greens :: Runalds Band";
        public override ItemDef InternalPickup => RoR2Content.Items.IceRing;

        public override string PickupText => "High damage hits also blast enemies with runic ice. Recharges over time.";

        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also blast enemies for <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage, plus an additional <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> base damage and <style=cIsUtility>slow</style> them down by <style=cIsUtility>45%</style> for <style=cIsUtility>3s</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.5f)]
        public static float totalDamage;

        [ConfigField("Base Damage", "Decimal.", 13f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 7f)]
        public static float damagePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(2.5f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = totalDamage;
                c.Index += 3;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return 1f;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Runalds's Band Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow80"),
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return 1f;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Runalds's Band Slow hook");
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var damageInfo = damageReport.damageInfo;
            var attacker = damageReport.attacker;
            if (!attacker)
            {
                return;
            }

            var victimBody = damageReport.victimBody;

            if (!victimBody)
            {
                return;
            }

            var attackerBody = damageReport.attackerBody;

            if (!attackerBody)
            {
                return;
            }

            var inventory = attackerBody.inventory;

            if (!inventory)
            {
                return;
            }

            if (damageInfo.procCoefficient > 0)
            {
                if (!damageInfo.procChainMask.HasProc(ProcType.Rings) && damageInfo.damage / attackerBody.damage >= 4f)
                {
                    if (attackerBody.HasBuff(RoR2Content.Buffs.ElementalRingsReady))
                    {
                        var procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.Rings);

                        var stack = inventory.GetItemCount(RoR2Content.Items.IceRing);
                        if (stack > 0)
                        {
                            DamageInfo baseDamageProc = new()
                            {
                                damage = attackerBody.damage * (baseDamage + damagePerStack * (stack - 1)),
                                damageColorIndex = DamageColorIndex.Item,
                                damageType = DamageType.Generic,
                                attacker = damageInfo.attacker,
                                crit = damageInfo.crit,
                                force = Vector3.zero,
                                inflictor = null,
                                position = damageInfo.position,
                                procChainMask = procChainMask,
                                procCoefficient = 0f
                            };

                            victimBody.healthComponent.TakeDamage(baseDamageProc);
                        }
                    }
                }
            }
        }
    }
}