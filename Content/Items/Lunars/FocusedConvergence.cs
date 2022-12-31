/*
using RoR2;
using System.Collections.ObjectModel;

namespace WellRoundedBalance.Items.Lunars
{
    public class FocusedConvergence : ItemBase
    {
        public static float Speed;
        public static float Radius;
        public static float ChargePerKill;
        public static float StackChargePerKill;
        public static int DifficultyWithReward;
        public static float RampUp;
        public static float Delay;
        public static int Cap;

        public override string Name => ":: Items ::::: Lunars :: Focused Convergence";
        public override string InternalPickupToken => "focusedConvergence";

        public override string PickupText => "Increase the speed of Teleporter charging... <color=#FF7F7F>BUT" + (ChargePerKill > 0 ? " increase its difficulty and" : "") +
                                             " reduce the size of the zone</color>.";

        // this is a complicated string to ? : so maybeeee pls someone help pls
        public override string DescText => (ChargePerKill > 0 ? "Teleporters gain <style=cIsUtility>" + d(ChargePerKill) + "</style>" + (StackChargePerKill > 0 ? " <style=cStack>(+" + d(StackChargePerKill) + " per stack)</style> <style=cIsUtility>charge on kill</style>." : " charge on kill</style>.") : " ") +
                                           (Speed > 0 ? "Teleporters charge <style=cIsUtility>" + d(Speed) + " <style=cStack>(+" + d(Speed) + " per stack)</style> faster</style>" : " ") +
                                           (DifficultyWithReward > 0 ? "Teleporters have <style=cIsHealth>" + DifficultyWithReward + "</style> <style=cStack>(+" + DifficultyWithReward + " per stack)</style>" + " <style=cIsHealth>mountain shrines</style> at all times" : " ") +
                                           (Radius > 0 ? ", but the size of the Teleporter zone is <style=cIsHealth>" + d(1 / Radius) + "</style> <style=cStack>(-" + d(1 / Radius) + " per stack)</style> smaller." : "");

        public override void Init()
        {
            Speed = ConfigOption(0.3f, "Charge Rate Increase", "Decimal. Vanilla is 0.3");
            Radius = ConfigOption(2f, "Radius Divisor", "Vanilla is 2");
            ChargePerKill = ConfigOption(0f, "Base Charge Percent On Kill", "Decimal. Vanilla is 0");
            StackChargePerKill = ConfigOption(0f, "Stack Charge Percent On Kill", "Decimal. Per Stack. Vanilla is 0");
            DifficultyWithReward = ConfigOption(0, "Mountain Shrines", "Per Stack. Vanilla is 0");
            Cap = ConfigOption(3, "Item Cap", "Vanilla is 3");
            RampUp = ConfigOption(5f, "Ramp Up Duration", "Vanilla is 5");
            Delay = ConfigOption(3f, "Delay", "Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.FocusConvergenceController.Awake += Changes;
            On.RoR2.GlobalEventManager.OnCharacterDeath += AddBehavior;
            On.RoR2.TeleporterInteraction.Start += AddDifficulty;
        }

        private void AddDifficulty(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            orig(self);
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
            if (stack > 0)
            {
                var bossGroup = self.bossGroup;
                var num = bossGroup.bonusRewardCount;
                bossGroup.bonusRewardCount = num + DifficultyWithReward * stack;
                num = self.shrineBonusStacks;
                self.shrineBonusStacks = num + DifficultyWithReward * stack;
            }
        }

        private void AddBehavior(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (damageReport.attacker && damageReport.attacker.GetComponent<CharacterBody>().inventory)
            {
                var inv = damageReport.attacker.GetComponent<CharacterBody>().inventory;
                var stack = inv.GetItemCount(RoR2Content.Items.FocusConvergence);
                var tp = TeleporterInteraction.instance;
                if (stack > 0 && tp.activationState == TeleporterInteraction.ActivationState.Charging && tp.monstersCleared && tp.holdoutZoneController.IsBodyInChargingRadius(damageReport.victimBody))
                {
                    tp.holdoutZoneController.charge += ChargePerKill + StackChargePerKill * (stack - 1);
                }
            }
            orig(self, damageReport);
        }

        private void Changes(On.RoR2.HoldoutZoneController.FocusConvergenceController.orig_Awake orig, UnityEngine.MonoBehaviour self)
        {
            orig(self);
            HoldoutZoneController.FocusConvergenceController.convergenceRadiusDivisor = Radius;
            HoldoutZoneController.FocusConvergenceController.convergenceChargeRateBonus = Speed;
            HoldoutZoneController.FocusConvergenceController.startupDelay = Delay;
            HoldoutZoneController.FocusConvergenceController.rampUpTime = RampUp;
        }
    }
}
*/