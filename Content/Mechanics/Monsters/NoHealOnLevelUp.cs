namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class NoHealOnLevelUp : MechanicBase<NoHealOnLevelUp>
    {
        public override string Name => ":: Mechanics ::::::::: Monster No Levelup Heal";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            var previousLevel = self.level;
            var previousHP = self.healthComponent.health;
            var previousShield = self.healthComponent.shield;
            orig(self);
            if (self.level > previousLevel)
            {
                if (self.teamComponent.teamIndex != TeamIndex.Player && self.healthComponent.combinedHealthFraction < 1f)
                {
                    if (self.healthComponent.health > previousHP)
                    {
                        self.healthComponent.health = previousHP;
                    }

                    if (self.healthComponent.shield > previousShield)
                    {
                        self.healthComponent.shield = previousShield;
                    }
                }
            }
        }
    }
}