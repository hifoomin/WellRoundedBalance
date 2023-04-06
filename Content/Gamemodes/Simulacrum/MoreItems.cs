using R2API.Utils;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class MoreItems : GamemodeBase<MoreItems>
    {
        public override string Name => ":: Gamemode :: Simulacrum More Enemy Items";

        [ConfigField("Item Gain", "Monsters Gain an item every Nth wave", 2)]
        public static int itemGain;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.InfiniteTowerRun.Start += InfiniteTowerRun_Start;
        }

        private void InfiniteTowerRun_Start(On.RoR2.InfiniteTowerRun.orig_Start orig, InfiniteTowerRun self)
        {
            orig(self);
            self.enemyItemPeriod = itemGain;
        }
    }
}