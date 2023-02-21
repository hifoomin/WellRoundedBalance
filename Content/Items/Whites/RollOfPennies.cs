using Inferno.Stat_AI;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class RollOfPennies : ItemBase
    {
        public override string Name => ":: Items : Whites :: Roll of Pennies";

        public override string InternalPickupToken => "goldOnHurt";

        public override string PickupText => "Gain gold at the beginning of each stage.";

        public override string DescText => "Gain <style=cIsUtility>10</style> gold on pickup. At the beginning of every stage, gain <style=cIsUtility>25 <style=cStack>(+15 per stack)</style> gold</style>. <style=cIsUtility>Scales over time.</style>";

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
                    TeamManager.instance.GiveTeamMoney(TeamIndex.Player, (uint)Run.instance.GetDifficultyScaledCost(10));
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
            if (stack > 0)
            {
                TeamManager.instance.GiveTeamMoney(TeamIndex.Player, (uint)Run.instance.GetDifficultyScaledCost(25 + 15 * (stack - 1)));
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "goldOnHurt"),
                x => x.MatchLdcI4(0)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, int.MaxValue);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Roll of Pennies Gold hook");
            }
        }
    }
}