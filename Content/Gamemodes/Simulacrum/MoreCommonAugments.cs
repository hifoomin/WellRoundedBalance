using R2API.Utils;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class MoreCommonAugments : GamemodeBase<MoreCommonAugments>
    {
        public override string Name => ":: Gamemode :: Simulacrum More Common Augments";

        [ConfigField("Default Wave Weight", "", 60)]
        public static int defaultWaveWeight;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.InfiniteTowerWaveCategory.SelectWavePrefab += InfiniteTowerWaveCategory_SelectWavePrefab;
        }

        private GameObject InfiniteTowerWaveCategory_SelectWavePrefab(On.RoR2.InfiniteTowerWaveCategory.orig_SelectWavePrefab orig, InfiniteTowerWaveCategory self, InfiniteTowerRun run, Xoroshiro128Plus rng)
        {
            self.wavePrefabs[0].weight = defaultWaveWeight;
            return orig(self, run, rng);
        }
    }
}