using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class Chronobauble
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var debuff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (stack > 0 && sender && debuff)
                {
                    args.attackSpeedMultAdd -= Main.ChronobaubleStacking.Value ? Main.ChronobaubleAS.Value * stack : Main.ChronobaubleAS.Value;
                }
            }
        }
    }
}
