using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class HarvestersScythe : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Harvesters Scythe";
        public override string InternalPickupToken => "healOnCrit";

        public override string PickupText => "'Backstabs' heal a percentage of missing health.";

        public override string DescText => "<style=cIsDamage>Backstabs</style> <style=cIsHealing>heal</style> for <style=cIsHealing>3%</style> <style=cStack>(+1.5% per stack)</style> of your <style=cIsHealing>missing health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            IL.RoR2.GlobalEventManager.OnCrit += GlobalEventManager_OnCrit;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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
                Main.WRBLogger.LogError("Failed to apply Harvester's Scythe Deletion 2 hook");
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
                Main.WRBLogger.LogError("Failed to apply Harvester's Scythe Deletion 1 hook");
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active)
            {
                var stack = characterBody.inventory.GetItemCount(RoR2Content.Items.HealOnCrit);
                characterBody.AddItemBehavior<HarvestersScytheController>(stack);
            }
        }
    }

    public class HarvestersScytheController : CharacterBody.ItemBehavior
    {
        private void Start()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var characterBody = attacker.GetComponent<CharacterBody>();
                if (stack > 0 && characterBody)
                {
                    var healthComponent = characterBody.healthComponent;
                    var vector = characterBody.corePosition - damageInfo.position;
                    ProcType procType = (ProcType)1239867;
                    if ((damageInfo.damageType & DamageType.DoT) != DamageType.DoT && (damageInfo.procChainMask.HasProc(procType) || BackstabManager.IsBackstab(-vector, self.body)))
                    {
                        damageInfo.damageColorIndex = DamageColorIndex.Fragile;
                        damageInfo.procChainMask.AddProc(procType);

                        if (NetworkServer.active)
                            healthComponent.Heal((healthComponent.fullHealth - healthComponent.health) * (0.03f + 0.015f * (stack - 1)) * damageInfo.procCoefficient, damageInfo.procChainMask, true);

                        Util.PlaySound("Play_item_proc_crit_heal", body.gameObject);
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}