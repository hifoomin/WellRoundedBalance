using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class Chronobauble
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (Main.ChronobaubleStacking.Value)
            {
                // best change i could come up with
                var debuff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (stack > 0)
                {
                    if (sender && debuff)
                    {
                        args.attackSpeedMultAdd -= Main.ChronobaubleAS.Value * stack;
                    }
                }
            }
            else
            {
                //best change i could come up with
                var debuff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (stack > 0)
                {
                    if (sender && debuff)
                    {
                        args.attackSpeedMultAdd -= Main.ChronobaubleAS.Value;
                    }
                    
                }
            }
        }
    }
}
