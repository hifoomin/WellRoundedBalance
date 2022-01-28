using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class BackupMag
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                if (stack > 0)
                {
                    args.secondaryCooldownMultAdd -= Main.BackupMagCDR.Value * stack;
                }
            }
        }
    }
}
