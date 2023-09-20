using MonoMod.Cil;

namespace WellRoundedBalance.Achievements.Commando
{
    internal class Incorruptible : AchievementBase<Incorruptible>
    {
        public override string Token => "commandoNonLunarEndurance";

        public override string Description => "As Commando, clear 11 stages in a single run without picking up any Lunar items.";

        public override string Name => ":: Achievements :: Survivor :: Annihilator";

        public override void Hooks()
        {
            On.RoR2.Achievements.Commando.CommandoNonLunarEnduranceAchievement.OnStatsChanged += CommandoNonLunarEnduranceAchievement_OnStatsChanged;
        }

        private void CommandoNonLunarEnduranceAchievement_OnStatsChanged(On.RoR2.Achievements.Commando.CommandoNonLunarEnduranceAchievement.orig_OnStatsChanged orig, RoR2.Achievements.Commando.CommandoNonLunarEnduranceAchievement self)
        {
            RoR2.Achievements.Commando.CommandoNonLunarEnduranceAchievement.requirement = 11;
            orig(self);
        }

        private void RailgunnerDealMassiveDamageAchievement_onClientDamageNotified(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1000000f)))
            {
                c.Next.Operand = 100000f;
            }
            else
            {
                Logger.LogError("Failed to apply Railgunner: Annihilator Damage hook");
            }
        }
    }
}