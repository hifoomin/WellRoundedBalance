namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class MonsterHealthBars : MechanicBase<MonsterHealthBars>
    {
        public override string Name => ":: Mechanics ::::::::: Monster Health Bars";

        [ConfigField("Health Bar Show Duration", "", float.MaxValue)]
        public static float showDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.UI.CombatHealthBarViewer.Awake += CombatHealthBarViewer_Awake;
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
        }

        private void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            var bossExists = self.currentBossGroup && self.currentBossGroup.combatSquad.memberCount > 0;
            self.container.SetActive(bossExists);
            if (bossExists)
            {
                var totalObservedHealth = self.currentBossGroup.totalObservedHealth;
                var totalMaxObservedMaxHealth = self.currentBossGroup.totalMaxObservedMaxHealth;
                var percentHealth = ((totalMaxObservedMaxHealth == 0f) ? 0f : Mathf.Clamp01(totalObservedHealth / totalMaxObservedMaxHealth));
                self.delayedTotalHealthFraction = Mathf.Clamp(Mathf.SmoothDamp(self.delayedTotalHealthFraction, percentHealth, ref self.healthFractionVelocity, 0.1f, float.PositiveInfinity, Time.deltaTime), percentHealth, 1f);
                self.fillRectImage.fillAmount = percentHealth;
                self.delayRectImage.fillAmount = self.delayedTotalHealthFraction;

                RoR2.UI.HUDBossHealthBarController.sharedStringBuilder.Clear().AppendUlong((ulong)totalObservedHealth, 1U, uint.MaxValue).Append("/")
                    .AppendUlong((ulong)totalMaxObservedMaxHealth, 1U, uint.MaxValue);

                self.healthLabel.SetText(RoR2.UI.HUDBossHealthBarController.sharedStringBuilder);
                self.bossNameLabel.SetText(self.currentBossGroup.bestObservedName, true);
                self.bossSubtitleLabel.SetText(self.currentBossGroup.bestObservedSubtitle, true);
            }
        }

        private void CombatHealthBarViewer_Awake(On.RoR2.UI.CombatHealthBarViewer.orig_Awake orig, RoR2.UI.CombatHealthBarViewer self)
        {
            orig(self);
            self.healthBarDuration = showDuration;
        }
    }
}