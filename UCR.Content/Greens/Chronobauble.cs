using R2API;
using RoR2;

namespace UltimateCustomRun
{
    static class Chronobauble
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (Main.ChronobaubleStacking.Value)
            {
                // best change i could come up with
                var buff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (buff && stack > 0)
                {
                    args.attackSpeedMultAdd -= Main.ChronobaubleAS.Value * stack;
                }
            }
            else
            {
                //best change i could come up with
                var buff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (buff && stack > 0)
                {
                    args.attackSpeedMultAdd -= Main.ChronobaubleAS.Value;
                }
            }
            // this method throws
        }
    }
}
