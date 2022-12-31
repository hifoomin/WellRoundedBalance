using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Whites
{
    public class BackupMag : ItemBase
    {
        public override string Name => ":: Items : Whites :: Backup Mag";
        public override string InternalPickupToken => "secondarySkillMagazine";

        public override string PickupText => "Add an extra charge of your Secondary skill and reduce its cooldown.";

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>. Reduce your <style=cIsUtility>secondary skill cooldown</style> by <style=cIsUtility>5%</style> <style=cStack>(+5% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                if (stack > 0)
                {
                    args.secondaryCooldownMultAdd -= 0.05f * stack;
                }
            }
        }
    }
}