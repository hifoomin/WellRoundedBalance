using RoR2;
using System.Collections.ObjectModel;

namespace WellRoundedBalance.Items.Lunars
{
    public class FocusedConvergence : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Focused Convergence";
        public override string InternalPickupToken => "focusedConvergence";

        public override string PickupText => "Teleporter events grant additional rewards... <color=#FF7F7F>BUT increase their difficulty.</color>";

        // this is a complicated string to ? : so maybeeee pls someone help pls
        public override string DescText => "Increase <style=cIsUtility>teleporter event rewards</style> by <style=cIsUtility>2</style> <style=cStack>(+2 per stack)</style>. Teleporter events spawn <style=cIsUtility>100%</style> <style=cStack>(+100% per stack)</style> more bosses.";

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
                if (characterMaster.teamIndex == TeamIndex.Player)
                {
                    stack += characterMaster.inventory.GetItemCount(RoR2Content.Items.FocusConvergence);
                }
            }
            orig(self);
            if (stack > 0)
            {
                // var bossGroup = self.bossGroup;
                self.shrineBonusStacks += 1 * stack;
                self.bossGroup.bonusRewardCount += 2 * stack;
            }
        }

        private void Changes(On.RoR2.HoldoutZoneController.FocusConvergenceController.orig_Awake orig, UnityEngine.MonoBehaviour self)
        {
            orig(self);
            HoldoutZoneController.FocusConvergenceController.convergenceRadiusDivisor = 1f;
            HoldoutZoneController.FocusConvergenceController.convergenceChargeRateBonus = 0f;
            HoldoutZoneController.FocusConvergenceController.startupDelay = 1 / 60f;
            HoldoutZoneController.FocusConvergenceController.rampUpTime = 1f;
        }
    }
}