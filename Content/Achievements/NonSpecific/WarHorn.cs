using RoR2.Achievements;

namespace WellRoundedBalance.Achievements.NonSpecific
{
    internal class WarHorn : AchievementBase<WarHorn>
    {
        public override string Token => "multiCombatShrine";

        public override string Description => "Complete 2 Combat Shrines in a single stage.";

        public override string Name => ":: Achievements : Non Specific :: Warmonger";

        public override void Hooks()
        {
            On.RoR2.Achievements.MultiCombatShrineAchievement.MultiCombatShrineServerAchievement.Check += MultiCombatShrineServerAchievement_Check;
        }

        private void MultiCombatShrineServerAchievement_Check(On.RoR2.Achievements.MultiCombatShrineAchievement.MultiCombatShrineServerAchievement.orig_Check orig, BaseServerAchievement self)
        {
            MultiCombatShrineAchievement.MultiCombatShrineServerAchievement.requirement = 2;
            orig(self);
        }
    }
}