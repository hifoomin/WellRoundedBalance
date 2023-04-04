using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Lunars
{
    public class FocusedConvergence : ItemBase<FocusedConvergence>
    {
        public override string Name => ":: Items ::::: Lunars :: Focused Convergence";
        public override ItemDef InternalPickup => RoR2Content.Items.FocusConvergence;

        public override string PickupText => "Teleporter events grant additional rewards... <color=#FF7F7F>BUT increase their difficulty.</color>";
        public override string DescText => "Increase <style=cIsUtility>teleporter event rewards</style> by <style=cIsUtility>" + mountainShrineCount + "</style> <style=cStack>(+" + mountainShrineCount + " per stack)</style>. Teleporter events are <style=cIsUtility>" + d(mountainShrineCount) + "</style> <style=cStack>(+" + d(mountainShrineCount) + " per stack)</style> harder.";

        [ConfigField("Mountain Shrine Count", 2)]
        public static int mountainShrineCount;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.FocusConvergenceController.Awake += Changes;
            IL.RoR2.HoldoutZoneController.FocusConvergenceController.ApplyRadius += FocusConvergenceController_ApplyRadius;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            On.RoR2.Inventory.RemoveItem_ItemIndex_int += Inventory_RemoveItem_ItemIndex_int;
            On.RoR2.Stage.Start += Stage_Start;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            var stack = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, false);
            if (NetworkServer.active && stack > 0)
            {
                var tp = TeleporterInteraction.instance;
                if (tp)
                {
                    for (int i = 0; i < stack * mountainShrineCount; i++)
                    {
                        tp.AddShrineStack();
                    }
                }
            }
        }

        private void Inventory_RemoveItem_ItemIndex_int(On.RoR2.Inventory.orig_RemoveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active && itemIndex == RoR2Content.Items.FocusConvergence.itemIndex)
            {
                var tp = TeleporterInteraction.instance;
                if (tp)
                {
                    if (tp.shrineBonusStacks - mountainShrineCount >= 0)
                        for (int i = 0; i < mountainShrineCount; i++)
                        {
                            tp.shrineBonusStacks--;
                        }
                }
            }
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active && itemIndex == RoR2Content.Items.FocusConvergence.itemIndex)
            {
                var tp = TeleporterInteraction.instance;
                if (tp)
                {
                    for (int i = 0; i < mountainShrineCount; i++)
                    {
                        tp.AddShrineStack();
                    }
                }
            }
        }

        private void FocusConvergenceController_ApplyRadius(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(HoldoutZoneController.FocusConvergenceController), "currentFocusConvergenceCount"),
                x => x.MatchConvR4()))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 1;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Focused Convergence Radius hook");
            }
        }

        private void Changes(On.RoR2.HoldoutZoneController.FocusConvergenceController.orig_Awake orig, MonoBehaviour self)
        {
            orig(self);
            HoldoutZoneController.FocusConvergenceController.convergenceRadiusDivisor = 1f;
            HoldoutZoneController.FocusConvergenceController.convergenceChargeRateBonus = 0f;
            HoldoutZoneController.FocusConvergenceController.startupDelay = 1 / 60f;
            HoldoutZoneController.FocusConvergenceController.rampUpTime = 1f;
        }
    }
}