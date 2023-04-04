namespace WellRoundedBalance.Items.Greens
{
    public class SquidPolyp : ItemBase<SquidPolyp>
    {
        public override string Name => ":: Items :: Greens :: Squid Polyp";
        public override ItemDef InternalPickup => RoR2Content.Items.Squid;

        public override string PickupText => "Activating an interactable summons a Squid Turret nearby.";
        public override string DescText => "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>" + lifetime + "</style> seconds.";

        [ConfigField("Lifetime", 45)]
        public static int lifetime;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            // I was told by IL gods that hooking the method isnt possible so lets do the stupiiiiiiiid
            On.RoR2.CharacterMaster.OnBodyStart += (orig, self, body) =>
            {
                orig(self, body);
                if (body.bodyIndex == BodyCatalog.FindBodyIndex("SquidTurretBody"))
                {
                    self.inventory.RemoveItem(RoR2Content.Items.HealthDecay, self.inventory.GetItemCount(RoR2Content.Items.HealthDecay));
                    self.inventory.GiveItem(RoR2Content.Items.HealthDecay, lifetime);
                }
            };
        }
    }
}