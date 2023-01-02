using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using On.EntityStates.MoonElevator;

namespace WellRoundedBalance.Mechanic.HoldoutZone {
    public class OptionalPillars : GlobalBase<OptionalPillars> {
        public override string Name => ":: Global ::: Holdout Zones ::: Optional Pillars";

        public override void Hooks()
        {
            On.RoR2.MoonBatteryMissionController.OnEnable += SkipPillars;
            Inactive.OnEnter += EnableElevators;
        }

        private void SkipPillars(On.RoR2.MoonBatteryMissionController.orig_OnEnable orig, MoonBatteryMissionController self) {
            self.enabled = false;
        }

        private void EnableElevators(Inactive.orig_OnEnter orig, EntityStates.MoonElevator.Inactive self) {
            self.outer.SetNextState(new EntityStates.MoonElevator.InactiveToReady());
        }
        
    }
}