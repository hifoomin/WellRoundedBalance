using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class LostSeersLenses : ItemBase<LostSeersLenses>
    {
        public override string Name => ":: Items :::::: Voids :: Lost Seers Lenses";
        public override ItemDef InternalPickup => DLC1Content.Items.CritGlassesVoid;

        public override string PickupText => StackDesc(chance, chanceStack, init => $"Gain a <style=cIsDamage>{d(init)}</style> chance to deal ", d) + "<style=cIsDamage>" + d(damage) + "</style> base damage. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
        public override string DescText => StackDesc(chance, chanceStack, init => $"Your attacks have a <style=cIsDamage>{d(init)}</style>{{Stack}} chance to deal ", d) + "<style=cIsDamage>" + d(damage) + "</style> base damage. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";

        [ConfigField("Base Chance", "Decimal.", 0.005f)]
        public static float chance;

        [ConfigField("Chance per Stackk", "Decimal.", 0.005f)]
        public static float chanceStack;

        [ConfigField("Chance is Hyperbolicc", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float chanceIsHyperbolic;

        [ConfigField("Champion Chance Multiplierr", "Decimal.", 1f)]
        public static float championMultiplier;

        [ConfigField("Damage", "Decimal.", 30f)]
        public static float damage;

        [ConfigField("Proc Coefficient", "Decimal.", 0f)]
        public static float procCoefficient;

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            Changes();
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var victimBody = report.victimBody;
            if (!victimBody)
            {
                return;
            }

            var victimHc = victimBody.healthComponent;
            if (!victimHc)
            {
                return;
            }

            var attacker = report.attacker;
            if (!attacker)
            {
                return;
            }

            var attackerBody = report.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var inventory = attackerBody.inventory;
            if (!inventory)
            {
                return;
            }

            var ch = StackAmount(chance, chanceStack, inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid), chanceIsHyperbolic);

            ProcType mask = (ProcType)58129798;
            if (Util.CheckRoll(ch * 100f, attackerBody.master) && !report.damageInfo.procChainMask.HasProc(mask) && report.damageInfo.procCoefficient > 0)
            {
                var vroggleVFX = HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab; // where did the r come from
                EffectManager.SpawnEffect(vroggleVFX, new EffectData                                // vroggle = void croggle = void crit goggle
                {
                    origin = victimBody.corePosition,
                    scale = victimBody.radius * 1.2f
                }, true);
                var pipeBomb = new DamageInfo()
                {
                    attacker = attacker,
                    crit = false,
                    damage = attackerBody.damage * damage,
                    damageType = DamageType.Generic,
                    inflictor = attacker,
                    procCoefficient = procCoefficient * Items.Greens._ProcCoefficients.globalProc,
                    damageColorIndex = DamageColorIndex.Void,
                    force = Vector3.zero,
                    position = victimBody.transform.position,
                };
                pipeBomb.procChainMask.AddProc(mask);
                victimHc.TakeDamage(pipeBomb);
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Items), "CritGlassesVoid")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Lost Seers Lenses Deletion hook");
            }
        }

        private void Changes()
        {
            var execute = Utils.Paths.GameObject.CritGlassesVoidExecuteEffect.Load<GameObject>();
            var getReal = execute.transform.GetChild(9);
            getReal.gameObject.SetActive(false);
        }
    }
}