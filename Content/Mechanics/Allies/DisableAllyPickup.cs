namespace WellRoundedBalance.Mechanics.Allies
{
    internal class DisableAllyPickup : MechanicBase
    {
        public override string Name => ":: Mechanics ::::::::::::::: Disable Ally Pickup";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.MoneyPickup.OnTriggerStay += MoneyPickup_OnTriggerStay;
            On.RoR2.AmmoPickup.OnTriggerStay += AmmoPickup_OnTriggerStay;
            On.RoR2.GravitatePickup.OnTriggerEnter += GravitatePickup_OnTriggerEnter;
        }

        private void GravitatePickup_OnTriggerEnter(On.RoR2.GravitatePickup.orig_OnTriggerEnter orig, GravitatePickup self, Collider other)
        {
            var body = other.gameObject.GetComponent<CharacterBody>();
            if (body && !body.isPlayerControlled && (self.transform.parent.name == "BonusMoneyPack(Clone)" || self.transform.parent.name == "AmmoPack(Clone)"))
            {
                return;
            }
            orig(self, other);
        }

        private void AmmoPickup_OnTriggerStay(On.RoR2.AmmoPickup.orig_OnTriggerStay orig, AmmoPickup self, Collider other)
        {
            var body = other.gameObject.GetComponent<CharacterBody>();
            if (body && !body.isPlayerControlled)
            {
                return;
            }
            orig(self, other);
        }

        private void MoneyPickup_OnTriggerStay(On.RoR2.MoneyPickup.orig_OnTriggerStay orig, MoneyPickup self, Collider other)
        {
            var body = other.gameObject.GetComponent<CharacterBody>();
            if (body && !body.isPlayerControlled)
            {
                return;
            }
            orig(self, other);
        }
    }
}