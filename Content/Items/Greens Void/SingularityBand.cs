using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class SingularityBand : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Singularity Band";
        public override string InternalPickupToken => "elementalRingVoid";

        public override string PickupText => "High damage hits also create unstable black holes. Recharges over time. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";

        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also fire a black hole that <style=cIsUtility>draws enemies within " + baseRadius + "m" +
                                           (radiusPerStack > 0 ? " <style=cStack>(+" + radiusPerStack + "m per stack)</style>" : "") + " into its center</style>. Lasts <style=cIsUtility>5</style> seconds before collapsing, dealing <style=cIsDamage>" + d(baseTotalDamage) + "</style> " +
                                           (totalDamagePerStack > 0 ? "<style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style>" : "") + "TOTAL damage. Recharges every <style=cIsUtility>" + cooldown + "</style> seconds. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";

        [ConfigField("Cooldown", 10f)]
        public static float cooldown;

        [ConfigField("Base TOTAL Damage", "Decimal.", 0.5f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal.", 0f)]
        public static float totalDamagePerStack;

        [ConfigField("Base Radius", "", 15f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", "", 7.5f)]
        public static float radiusPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "ElementalRingVoidReady")))
            {
                c.Remove();
                c.Emit<Buffs.Useless>(OpCodes.Ldsfld, nameof(Buffs.Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Singularity Band Deletion hook");
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            if (!damageReport.attacker)
            {
                return;
            }

            var body = damageReport.attackerBody;

            if (!body)
            {
                return;
            }

            var inventory = body.inventory;

            if (!inventory)
            {
                return;
            }

            if (damageReport.damageInfo.procCoefficient > 0)
            {
                if (!damageReport.damageInfo.procChainMask.HasProc(ProcType.Rings) && damageReport.damageInfo.damage / damageReport.attackerBody.damage >= 4f)
                {
                    if (body.HasBuff(DLC1Content.Buffs.ElementalRingVoidReady))
                    {
                        body.RemoveBuff(DLC1Content.Buffs.ElementalRingVoidReady);
                        var buffCount = 1;
                        while (buffCount <= cooldown)
                        {
                            body.AddTimedBuff(DLC1Content.Buffs.ElementalRingVoidCooldown, buffCount);
                            buffCount++;
                        }

                        var procMask = damageReport.damageInfo.procChainMask;
                        procMask.AddProc(ProcType.Rings);

                        var stack = inventory.GetItemCount(DLC1Content.Items.ElementalRingVoid);
                        if (stack > 0)
                        {
                            var singularity = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ElementalRingVoidBlackHole");
                            var radialForce = singularity.GetComponent<RadialForce>();
                            var projectileExplosion = singularity.GetComponent<ProjectileExplosion>();

                            var scale = (baseRadius + radiusPerStack * (stack - 1)) / 15f;
                            singularity.transform.localScale = new Vector3(scale, scale, scale);
                            radialForce.radius = baseRadius + radiusPerStack * (stack - 1);
                            projectileExplosion.blastRadius = baseRadius + radiusPerStack * (stack - 1);

                            float damage = baseTotalDamage + totalDamagePerStack * (stack - 1);
                            float totalDamage = Util.OnHitProcDamage(damageReport.damageInfo.damage, body.damage, damage);

                            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                            {
                                damage = totalDamage,
                                crit = damageReport.damageInfo.crit,
                                damageColorIndex = DamageColorIndex.Void,
                                position = damageReport.damageInfo.position,
                                procChainMask = procMask,
                                force = 6000f,
                                owner = damageReport.damageInfo.attacker,
                                projectilePrefab = singularity,
                                rotation = Quaternion.identity,
                                target = null
                            });
                        }
                    }
                }
            }
        }
    }
}