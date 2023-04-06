using R2API.Utils;
using System;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class BetterBlacklist : GamemodeBase<BetterBlacklist>
    {
        public override string Name => ":: Gamemode :: Simulacrum Better Blacklist";

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
            self.blacklistedItems = new ItemDef[] { RoR2Content.Items.FocusConvergence, RoR2Content.Items.LunarTrinket, RoR2Content.Items.TitanGoldDuringTP };
            self.blacklistedTags = new ItemTag[] { ItemTag.ObliterationRelated };
        }
    }
}