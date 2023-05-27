using Rewired.ComponentControls.Data;
using RoR2.Achievements;
using System;

namespace WellRoundedBalance.Achievements.NonSpecific
{
    internal class BundleOfFireworks : AchievementBase
    {
        public override string Token => "repeatedlyDuplicateItems";

        public override string Description => "Duplicate the same item " + Mathf.Min(Interactables.AllPrinters.maxCommonUses, 7) + " times in a row with a 3D Printer.";

        public override string Name => ":: Achievements : Non Specific :: Maybe One More";

        public override void Hooks()
        {
            IL.RoR2.Achievements.RepeatedlyDuplicateItemsAchievement.RepeatedlyDuplicateItemsServerAchievement.OnItemSpentOnPurchase += RepeatedlyDuplicateItemsServerAchievement_OnItemSpentOnPurchase;
        }

        private void RepeatedlyDuplicateItemsServerAchievement_OnItemSpentOnPurchase(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(7)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return Mathf.Min(Interactables.AllPrinters.maxCommonUses, 7);
                });
            }
            else
            {
                Logger.LogError("Failed to apply Maybe One More Item Count hook");
            }
        }
    }
}