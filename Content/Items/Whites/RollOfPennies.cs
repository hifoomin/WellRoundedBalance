using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class RollOfPennies : ItemBase<RollOfPennies>
    {
        public override string Name => ":: Items : Whites :: Roll of Pennies";

        public override ItemDef InternalPickup => DLC1Content.Items.GoldOnHurt;

        public override string PickupText => "Gain gold at the beginning of each stage.";

        public override string DescText =>
            StackDesc(goldOnPickup, goldOnPickupStack, init => $"Gain <style=cIsUtility>{init}</style>{{Stack}} gold on pickup. ", noop) +
            StackDesc(baseGoldPerStage, goldPerStagePerStack, init => $"At the beginning of every stage, gain <style=cIsUtility>{init}</style>{{Stack}} gold</style>.", noop) + (scaleOverTime ? " <style=cIsUtility>Scales over time.</style>" : "");

        [ConfigField("Gold On Pickup", 10)]
        public static int goldOnPickup;

        [ConfigField("Gold On Pickup per Stack", 0)]
        public static int goldOnPickupStack;

        [ConfigField("Gold On Pickup is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float goldOnPickupIsHyperbolic;

        [ConfigField("Base Gold Per Stage", 25)]
        public static int baseGoldPerStage;

        [ConfigField("Gold Per Stage Per Stack", 25)]
        public static int goldPerStagePerStack;

        [ConfigField("Gold Per Stage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float goldPerStageIsHyperbolic;

        [ConfigField("Scales Over Time", true)]
        public static bool scaleOverTime;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            On.RoR2.Stage.Start += Stage_Start;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            var masters = CharacterMaster.readOnlyInstancesList;

            var stack = Util.GetItemCountForTeam(TeamIndex.Player, DLC1Content.Items.GoldOnHurt.itemIndex, false);

            // Logger.LogError("stack count is " + stack);

            if (stack > 0 && NetworkServer.active)
            {
                foreach (CharacterMaster master in masters)
                {
                    // Logger.LogError("master is " + master);
                    if (master.playerCharacterMasterController)
                    {
                        // Logger.LogError("Iterating through " + master + " to give gold");
                        uint ret = (uint)StackAmount(baseGoldPerStage, goldPerStagePerStack, stack, goldPerStageIsHyperbolic);
                        if (scaleOverTime) ret = (uint)Run.instance.GetDifficultyScaledCost((int)ret);
                        ret = (uint)Mathf.CeilToInt(ret);
                        // Logger.LogError("ret for pennies is " + ret);
                        master.GiveMoney(ret);
                    }
                }
            }
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active)
            {
                if (itemIndex == DLC1Content.Items.GoldOnHurt.itemIndex)
                {
                    uint ret = (uint)StackAmount(goldOnPickup, goldOnPickupStack, count, goldOnPickupIsHyperbolic);
                    if (scaleOverTime) ret = (uint)Run.instance.GetDifficultyScaledCost((int)ret);
                    TeamManager.instance.GiveTeamMoney(TeamIndex.Player, ret);
                }
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), nameof(HealthComponent.ItemCounts.goldOnHurt))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4_0);
            }
            else Logger.LogError("Failed to apply Roll of Pennies Gold hook");
        }
    }
}