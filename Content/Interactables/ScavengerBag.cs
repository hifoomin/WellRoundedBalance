namespace WellRoundedBalance.Interactables
{
    public class ScavengerBag : InteractableBase
    {
        public override string Name => ":: Interactables :::::::: Scavenger Bag";

        [ConfigField("Item Drop Count", "", 3)]
        public static int itemDropCount;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ScavBackpack.Opening.OnEnter += Opening_OnEnter;
        }

        private void Opening_OnEnter(On.EntityStates.ScavBackpack.Opening.orig_OnEnter orig, EntityStates.ScavBackpack.Opening self)
        {
            if (self.characterBody.bodyIndex == BodyCatalog.FindBodyIndexCaseInsensitive("scavlunar"))
            {
                EntityStates.ScavBackpack.Opening.maxItemDropCount = 10;
            }
            else
            {
                EntityStates.ScavBackpack.Opening.maxItemDropCount = itemDropCount;
            }

            orig(self);
        }
    }
}