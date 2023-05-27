namespace WellRoundedBalance.Mechanics.Director
{
    internal class CombatDirector : MechanicBase<CombatDirector>
    {
        public override string Name => ":: Mechanics :::: Combat Director";

        [ConfigField("Minimum Reroll Spawn Interval Multiplier", "", 1.8f)]
        public static float minimumRerollSpawnIntervalMultiplier;

        [ConfigField("Credit Multiplier", "", 1.35f)]
        public static float creditMultiplier;

        [ConfigField("Elite Bias Multiplier", "", 0.9f)]
        public static float eliteBiasMultiplier;

        [ConfigField("Credit Multiplier for each Mountain Shrine", "", 1.05f)]
        public static float creditMultiplierForEachMountainShrine;

        [ConfigField("Gold and Experience Multiplier for each Mountain Shrine", "", 0.9f)]
        public static float goldAndExperienceMultiplierForEachMountainShrine;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            On.RoR2.CombatDirector.OnDisable += CombatDirector_OnDisable;
            On.RoR2.CombatDirector.Spawn += CombatDirector_Spawn;
        }

        private bool CombatDirector_Spawn(On.RoR2.CombatDirector.orig_Spawn orig, RoR2.CombatDirector self, SpawnCard spawnCard, EliteDef eliteDef, Transform spawnTarget, DirectorCore.MonsterSpawnDistance spawnDistance, bool preventOverhead, float valueMultiplier, DirectorPlacementRule.PlacementMode placementMode)
        {
            self.monsterCredit += (0.3f * Mathf.Sqrt(Run.instance.difficultyCoefficient)) + ((spawnCard.directorCreditCost - 50) / 50);
            return orig(self, spawnCard, eliteDef, spawnTarget, spawnDistance, preventOverhead, valueMultiplier, placementMode);
        }

        private void CombatDirector_OnDisable(On.RoR2.CombatDirector.orig_OnDisable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval *= minimumRerollSpawnIntervalMultiplier;
            self.maxRerollSpawnInterval *= minimumRerollSpawnIntervalMultiplier;
            orig(self);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, RoR2.CombatDirector self)
        {
            self.maximumNumberToSpawnBeforeSkipping = 3;
            self.maxConsecutiveCheapSkips = 4;
            self.minRerollSpawnInterval /= minimumRerollSpawnIntervalMultiplier;
            self.maxRerollSpawnInterval /= minimumRerollSpawnIntervalMultiplier;
            self.creditMultiplier += (1f - creditMultiplier);
            self.eliteBias *= eliteBiasMultiplier;
            var teleporter = TeleporterInteraction.instance;
            if (teleporter != null)
            {
                for (int i = 0; i < teleporter.shrineBonusStacks; i++)
                {
                    self.creditMultiplier *= creditMultiplierForEachMountainShrine * Mathf.Pow(Run.instance.participatingPlayerCount, 0.05f);
                    self.expRewardCoefficient *= goldAndExperienceMultiplierForEachMountainShrine;
                    self.goldRewardCoefficient *= goldAndExperienceMultiplierForEachMountainShrine;
                }
            }
            orig(self);
        }
    }
}