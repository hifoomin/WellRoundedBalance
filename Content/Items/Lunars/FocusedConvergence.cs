using RoR2;
using System.Collections.ObjectModel;

namespace WellRoundedBalance.Items.Lunars
{
    public class FocusedConvergence : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Focused Convergence";
        public override string InternalPickupToken => "focusedConvergence";

        public override string PickupText => "Teleporter events grant additional rewards... <color=#FF7F7F>BUT increase their difficulty.</color>";
        public override string DescText => "Increase <style=cIsUtility>teleporter event rewards</style> by <style=cIsUtility>2</style> <style=cStack>(+2 per stack)</style>. Teleporter events are <style=cIsUtility>200%</style> <style=cStack>(+200% per stack)</style> harder.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.FocusConvergenceController.Awake += Changes;
            On.RoR2.TeleporterInteraction.Start += AddDifficulty;
        }

        private void AddDifficulty(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            int stack = 0;
            ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.inventory) // funnier eulogy, no team check
                {
                    stack += characterMaster.inventory.GetItemCount(RoR2Content.Items.FocusConvergence);
                }
            }
            if (NetworkServer.active && stack > 0)
            {
                for (int i = 0; i < stack * 2; i++)
                {
                    self.AddShrineStack();
                }
                /*
                self.shrineBonusStacks += 1 * stack;
                self.bossGroup.bonusRewardCount += 2 * stack;
                */
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