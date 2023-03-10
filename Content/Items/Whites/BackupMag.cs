namespace WellRoundedBalance.Items.Whites
{
    public class BackupMag : ItemBase
    {
        public override string Name => ":: Items : Whites :: Backup Mag";
        public override string InternalPickupToken => "secondarySkillMagazine";

        public override string PickupText => "Add an extra charge of your Secondary skill and reduce its cooldown.";

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>." +
            StackDesc(secondarySkillCooldownReduction, secondarySkillCooldownReductionStack, init => $" Reduce your <style=cIsUtility>secondary skill cooldown</style> by <style=cIsUtility>{d(init)}</style>{{Stack}}.", d);

        [ConfigField("Secondary Skill Cooldown Reduction", "Decimal.", 0.05f)]
        public static float secondarySkillCooldownReduction;

        [ConfigField("Secondary Skill Cooldown Reduction per Stack", "Decimal.", 0.05f)]
        public static float secondarySkillCooldownReductionStack;

        [ConfigField("Secondary Skill Cooldown Reduction is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float secondarySkillCooldownReductionIsHyperbolic;

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
                args.secondaryCooldownMultAdd -= StackAmount(secondarySkillCooldownReduction, secondarySkillCooldownReductionStack,
                    sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine), secondarySkillCooldownReductionIsHyperbolic);
            }
        }
    }
}