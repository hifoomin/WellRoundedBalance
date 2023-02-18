using On.EntityStates.MoonElevator;

namespace WellRoundedBalance.Mechanics.HoldoutZone
{
    public class OptionalPillars : MechanicBase<OptionalPillars>
    {
        public override string Name => ":: Mechanics :::::: Optional Pillars";

        public override void Hooks()
        {
            On.RoR2.MoonBatteryMissionController.OnEnable += SkipPillars;
            Inactive.OnEnter += EnableElevators;
        }

        private void SkipPillars(On.RoR2.MoonBatteryMissionController.orig_OnEnable orig, MoonBatteryMissionController self)
        {
            self.enabled = false;
        }

        private void EnableElevators(Inactive.orig_OnEnter orig, EntityStates.MoonElevator.Inactive self)
        {
            self.outer.SetNextState(new EntityStates.MoonElevator.InactiveToReady());
        }
    }
}