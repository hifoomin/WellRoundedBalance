namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class FasterWaves : GamemodeBase<FasterWaves>
    {
        public override string Name => ":: Gamemode :: Simulacrum Faster Waves";

        [ConfigField("Crab Speed Multiplier", "", 1.5f)]
        public static float crabSpeedMultiplier;

        [ConfigField("Fog Speed Multiplier", "", 1.6f)]
        public static float fogSpeedMultiplier;

        [ConfigField("Instant Wave On Potential Interact?", "", true)]
        public static bool instantWave;

        [ConfigField("Wave Timer", "", 15)]
        public static int waveTimer;

        [ConfigField("Enemy Spawn Grace Period", "", 0.6f)]
        public static float gracePeriod;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            On.RoR2.InfiniteTowerWaveController.Initialize += InfiniteTowerWaveController_Initialize;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnEnter += Travelling_OnEnter;
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

        
        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject gameObject)
        {
            if (instantWave)
            {
                if (Run.instance is InfiniteTowerRun itRun && gameObject.name == "OptionPickup(Clone)" && NetworkServer.active)
                {
                    itRun.waveController.OnTimerExpire();
                }
            }
        }
    }
}