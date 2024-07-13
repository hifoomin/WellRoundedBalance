namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class FasterWaves : GamemodeBase<FasterWaves>
    {
        public override string Name => ":: Gamemode :: Simulacrum Faster Waves";

        [ConfigField("Crab Speed Multiplier", "", 1.5f)]
        public static float crabSpeedMultiplier;

        [ConfigField("Fog Speed Multiplier", "", 1.6f)]
        public static float fogSpeedMultiplier;

        [ConfigField("Instant Wave On Picking an item?", "", true)]
        public static bool instantWave;

        [ConfigField("Wave Timer", "", 25)]
        public static int waveTimer;

        [ConfigField("Enemy Spawn Grace Period", "", 0.6f)]
        public static float gracePeriod;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            // GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            On.RoR2.PickupPickerController.SubmitChoice += PickupPickerController_SubmitChoice;
            On.RoR2.InfiniteTowerWaveController.Initialize += InfiniteTowerWaveController_Initialize;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnEnter += Travelling_OnEnter;
            On.EntityStates.InfiniteTowerSafeWard.Unburrow.OnEnter += Unburrow_OnEnter;
        }

        private void PickupPickerController_SubmitChoice(On.RoR2.PickupPickerController.orig_SubmitChoice orig, PickupPickerController self, int choiceIndex)
        {
            orig(self, choiceIndex);
            if (instantWave && NetworkServer.active)
            {
                if (Run.instance is InfiniteTowerRun run && self.gameObject.name == "OptionPickup(Clone)")
                {
                    run.waveController.OnTimerExpire();
                }
            }
        }

        private void Unburrow_OnEnter(On.EntityStates.InfiniteTowerSafeWard.Unburrow.orig_OnEnter orig, EntityStates.InfiniteTowerSafeWard.Unburrow self)
        {
            self.duration = waveTimer;
            orig(self);
        }

        private void Travelling_OnEnter(On.EntityStates.InfiniteTowerSafeWard.Travelling.orig_OnEnter orig, EntityStates.InfiniteTowerSafeWard.Travelling self)
        {
            self.travelSpeed = 5f * crabSpeedMultiplier;
            self.travelHeight = 10f * crabSpeedMultiplier;
            self.pathMaxSpeed = 7f * crabSpeedMultiplier;
            self.pathMaxJumpHeight = 15f * crabSpeedMultiplier;

            orig(self);
        }

        private void InfiniteTowerWaveController_Initialize(On.RoR2.InfiniteTowerWaveController.orig_Initialize orig, InfiniteTowerWaveController self, int waveIndex, Inventory enemyInventory, GameObject spawnTarget)
        {
            orig(self, waveIndex, enemyInventory, spawnTarget);
            self.squadDefeatGracePeriod = gracePeriod;
            self.suddenDeathRadiusConstrictingPerSecond = 0.05f * fogSpeedMultiplier;
            self.secondsBeforeSuddenDeath = 60f / fogSpeedMultiplier;
            self.secondsBeforeFailsafe = 60f / fogSpeedMultiplier;
            self.secondsAfterWave = waveTimer;
        }
    }
}