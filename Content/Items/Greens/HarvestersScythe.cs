using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class HarvestersScythe : ItemBase<HarvestersScythe>
    {
        public override string Name => ":: Items :: Greens :: Harvesters Scythe";
        public override ItemDef InternalPickup => RoR2Content.Items.HealOnCrit;

        public override string PickupText => "'Backstabs' heal a percentage of missing health.";

        public override string DescText => "<style=cIsDamage>Backstabs</style> <style=cIsHealing>heal</style> for <style=cIsHealing>" + d(baseMissingHealthHealingPercent) + "</style> <style=cStack>(+" + d(missingHealthHealingPercentPerStack) + " per stack)</style> of your <style=cIsHealing>missing health</style>.";

        private static ProcType Backstab = (ProcType)38921;

        [ConfigField("Base Missing Health Healing Percent", "Decimal. Formula for healing: (Maximum Health - Current Health) * (Base Missing Health Healing Percent + (Missing Health Healing Percent Per Stack * (Harvesters Scythe - 1)))", 0.018f)]
        public static float baseMissingHealthHealingPercent;

        [ConfigField("Missing Health Healing Percent Per Stack", "Decimal. Formula for healing: (Maximum Health - Current Health) * (Base Missing Health Healing Percent + (Missing Health Healing Percent Per Stack * (Harvesters Scythe - 1)))", 0.009f)]
        public static float missingHealthHealingPercentPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCrit += GlobalEventManager_OnCrit;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(13),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f)))
            {
                c.Index += 4;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Harvester's Scythe Deletion 2 hook");
            }
        }

        private void GlobalEventManager_OnCrit(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "HealOnCrit")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Harvester's Scythe Deletion 1 hook");
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            orig(self, info);

            if (!info.attacker || info.procChainMask.HasProc(Backstab))
            {
                return;
            }

            CharacterBody attacker = info.attacker.GetComponent<CharacterBody>();

            if (!attacker || info.damageType.HasFlag(DamageType.DoT))
            {
                return;
            }

            var inventory = attacker.inventory;
            if (!inventory)
            {
                return;
            }

            var stack = inventory.GetItemCount(RoR2Content.Items.HealOnCrit);

            var vector = (attacker.corePosition - info.position) * -1;

            if (BackstabManager.IsBackstab(vector, self.body) && stack > 0)
            {
                info.damageColorIndex = DamageColorIndex.WeakPoint;
                info.procChainMask.AddProc(Backstab);

                var healthComponent = attacker.healthComponent;

                var healing = (healthComponent.fullHealth - healthComponent.health) * (baseMissingHealthHealingPercent + missingHealthHealingPercentPerStack * (stack - 1));

                if (NetworkServer.active)
                {
                    healthComponent.Heal(healing, info.procChainMask, true);
                }
            }
        }
    }
}