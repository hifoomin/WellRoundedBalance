namespace WellRoundedBalance.Items.Reds
{
    public class Aegis : ItemBase<Aegis>
    {
        public override string Name => ":: Items ::: Reds :: Aegis";
        public override ItemDef InternalPickup => RoR2Content.Items.BarrierOnOverHeal;

        public override string PickupText => "Activaitng an interactable grants temporary barrier. Barrier doesn't decay while outside danger.";

        public override string DescText => "Activating an interactable grants a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>" + d(baseBarrierPercent) + "</style> <style=cStack>(+" + d(barrierPercentPerStack) + ")</style> of <style=cIsHealing>maximum health</style> While outside of danger, <style=cIsUtility>barrier will not decay</style>.";

        [ConfigField("Base Barrier Percent", "Decimal.", 0.1f)]
        public static float baseBarrierPercent;

        [ConfigField("Barrier Percent Per Stack", "Decimal.", 0.1f)]
        public static float barrierPercentPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
        {
            if (!interactableObject)
            {
                return;
            }
            if (!interactor)
            {
                return;
            }

            var body = interactor.GetComponent<CharacterBody>();
            if (!body)
            {
                return;
            }

            var inventory = body.inventory;
            if (!inventory)
            {
                return;
            }

            var stack = body.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
            if (stack <= 0)
            {
                return;
            }

            var hc = body.healthComponent;

            hc.AddBarrier(hc.fullCombinedHealth * (baseBarrierPercent + barrierPercentPerStack * (stack - 1)));
        }

        private void HealthComponent_Heal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "barrierOnOverHeal"),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Index += 2;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Aegis Overheal hook");
            }
        }
    }
}