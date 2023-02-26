namespace WellRoundedBalance.Items.Whites
{
    public class BackupMag : ItemBase
    {
        public override string Name => ":: Items : Whites :: Backup Mag";
        public override string InternalPickupToken => "secondarySkillMagazine";

        public override string PickupText => "Add an extra charge of your Secondary skill and reduce its cooldown.";

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>." +
                                           (secondarySkillCooldownReduction > 0 ? "Reduce your <style=cIsUtility>secondary skill cooldown</style> by <style=cIsUtility>" + d(secondarySkillCooldownReduction) + "</style> <style=cStack>(+" + d(secondarySkillCooldownReduction) + " per stack)</style>." : "");

        [ConfigField("Secondary Skill Cooldown Reduction", "Decimal.", 0.05f)]
        public static float secondarySkillCooldownReduction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                if (stack > 0)
                {
                    args.secondaryCooldownMultAdd -= secondarySkillCooldownReduction * stack;
                }
            }
        }
    }
}