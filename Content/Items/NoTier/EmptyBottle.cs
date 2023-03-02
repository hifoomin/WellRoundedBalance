using WellRoundedBalance.Items.Whites;

namespace WellRoundedBalance.Items.NoTier
{
    public static class EmptyBottle
    {
        public static void Init()
        {
            if (PowerElixir.refillEveryStage)
            {
                LanguageAPI.Add("ITEM_HEALINGPOTIONCONSUMED_PICKUP", "An empty container for an Elixir. Magically refills each stage.");
            }
        }
    }
}