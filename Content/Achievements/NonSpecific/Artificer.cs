namespace WellRoundedBalance.Achievements.NonSpecific
{
    internal class Artificer : AchievementBase<Artificer>
    {
        public override string Token => "freeMage";

        public override string Description => "Free the survivor suspended in time.";

        public override string Name => ":: Achievements : Non Specific :: Pause";

        [ConfigField("Lunar Coin Cost", "", 6)]
        public static int lunarCoinCost;

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var arti = Utils.Paths.GameObject.LockedMage.Load<GameObject>();
            var purchaseInteraction = arti.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost = lunarCoinCost;
        }
    }
}