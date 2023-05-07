namespace WellRoundedBalance.Interactables
{
    public class ScavengerBag : InteractableBase<ScavengerBag>
    {
        public override string Name => ":: Interactables :::::: Scavenger Bag";

        [ConfigField("Item Drop Count", "", 3)]
        public static int itemDropCount;

        [ConfigField("Lunar Coin Drop Count", "Only applies to Twisted Scavengers.", 10)]
        public static int lunarCoinDropCount;

        [ConfigField("Enable Infinite Scavenger Bags?", "", true)]
        public static bool infiniteBags;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ScavBackpack.Opening.OnEnter += Opening_OnEnter;
            On.EntityStates.ScavMonster.Death.OnEnter += Death_OnEnter;
        }

        private void Death_OnEnter(On.EntityStates.ScavMonster.Death.orig_OnEnter orig, EntityStates.ScavMonster.Death self)
        {
            if (NetworkServer.active)
            {
                Stage.instance.scavPackDroppedServer = false;
            }
            orig(self);
            if (NetworkServer.active)
            {
                Stage.instance.scavPackDroppedServer = false;
            }
        }

        private void Opening_OnEnter(On.EntityStates.ScavBackpack.Opening.orig_OnEnter orig, EntityStates.ScavBackpack.Opening self)
        {
            if (self.outer.name.Contains("ScavLunar"))
            {
                EntityStates.ScavBackpack.Opening.maxItemDropCount = lunarCoinDropCount;
            }
            else
            {
                EntityStates.ScavBackpack.Opening.maxItemDropCount = itemDropCount;
            }

            orig(self);
        }
    }
}