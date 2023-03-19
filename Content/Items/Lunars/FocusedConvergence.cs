using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Lunars
{
    public class FocusedConvergence : ItemBase
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
            On.RoR2.TeleporterInteraction.Start += AddDifficulty;
            IL.RoR2.HoldoutZoneController.FocusConvergenceController.ApplyRadius += FocusConvergenceController_ApplyRadius;
        }

        private void FocusConvergenceController_ApplyRadius(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(HoldoutZoneController.FocusConvergenceController), "currentFocusConvergenceCount")))
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

        private void AddDifficulty(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            var stack = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, false);
            if (NetworkServer.active && stack > 0)
            {
                for (int i = 0; i < stack * mountainShrineCount; i++)
                {
                    self.AddShrineStack();
                }
            }
            orig(self);
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