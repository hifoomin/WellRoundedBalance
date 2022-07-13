using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class SquidPolyp : ItemBase
    {
        public static int AttackSpeed;
        public static int Duration;

        public override string Name => ":: Items :: Greens :: Squid Polyp";
        public override string InternalPickupToken => "squidTurret";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>" + (10 * AttackSpeed) + "% <style=cStack>(+" + (10 * AttackSpeed) + "% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>" + Duration + "</style> seconds.";

        public override void Init()
        {
            AttackSpeed = ConfigOption(10, "Attack Speed Item", "Per Stack. Vanilla is Attack Speed Item * 10 = 100%.");
            ROSOption("Greens", 0f, 30f, 1f, "2");
            Duration = ConfigOption(30, "Lifetime", "Vanilla is 30.");
            ROSOption("Greens", 0f, 120f, 1f, "2");
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
                    self.inventory.GiveItem(RoR2Content.Items.HealthDecay, Duration);

                    self.inventory.RemoveItem(RoR2Content.Items.BoostAttackSpeed, self.inventory.GetItemCount(RoR2Content.Items.BoostAttackSpeed));
                    self.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, AttackSpeed);
                }
            };
        }
    }
}