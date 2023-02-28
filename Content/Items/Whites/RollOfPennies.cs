using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class RollOfPennies : ItemBase
    {
        public override string Name => ":: Items : Whites :: Roll of Pennies";

        public override string InternalPickupToken => "goldOnHurt";

        public override string PickupText => "Gain gold at the beginning of each stage.";

        public override string DescText => StackDesc(goldOnPickup, goldOnPickupStack, 
            init => $"Gain <style=cIsUtility>{init}</style>{{Stack}} gold on pickup. ",
            stack => stack.ToString()) + StackDesc(baseGoldPerStage, goldPerStagePerStack,
                init => $"At the beginning of every stage, gain <style=cIsUtility>{init}</style>{{Stack}} gold</style>. ",
                stack => stack.ToString()) + "<style=cIsUtility>Scales over time.</style>";
            
        [ConfigField("Gold On Pickup", "", 10)]
        public static int goldOnPickup;

        [ConfigField("Gold On Pickup per Stack", "", 0)]
        public static int goldOnPickupStack;

        [ConfigField("Base Gold Per Stage", "", 25)]
        public static int baseGoldPerStage;

        [ConfigField("Gold Per Stage Per Stack", "", 15)]
        public static int goldPerStagePerStack;

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

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active)
            {
                if (itemIndex == DLC1Content.Items.GoldOnHurt.itemIndex)
                {
                    TeamManager.instance.GiveTeamMoney(TeamIndex.Player, (uint)Run.instance.GetDifficultyScaledCost((int)StackAmount(goldOnPickup, goldOnPickupStack, count)));
                }
            }
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            int stack = 0;
            var readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.inventory)
                {
                    stack += characterMaster.inventory.GetItemCount(DLC1Content.Items.GoldOnHurt);
                }
            }
            TeamManager.instance.GiveTeamMoney(TeamIndex.Player, (uint)Run.instance.GetDifficultyScaledCost((int)StackAmount(baseGoldPerStage, goldPerStagePerStack, stack)));
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), nameof(HealthComponent.ItemCounts.goldOnHurt))) && c.TryGotoNext(x => x.MatchBle(out _)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, int.MaxValue);
            }
            else Main.WRBLogger.LogError("Failed to apply Roll of Pennies Gold hook");
        }
    }
}