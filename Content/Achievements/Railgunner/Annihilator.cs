using MonoMod.Cil;

namespace WellRoundedBalance.Achievements.Railgunner
{
    internal class Annihilator : AchievementBase<Annihilator>
    {
        public override string Token => "railgunnerDealMassiveDamage";

        public override string Description => "As Railgunner, deal 100,000 damage in one shot.";

        public override string Name => ":: Achievements :: Survivor :: Annihilator";

        public override void Hooks()
        {
            IL.RoR2.Achievements.Railgunner.RailgunnerDealMassiveDamageAchievement.onClientDamageNotified += RailgunnerDealMassiveDamageAchievement_onClientDamageNotified;
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