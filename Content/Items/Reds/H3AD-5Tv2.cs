﻿namespace WellRoundedBalance.Items.Reds
{
    public class Headstompers : ItemBase<Headstompers>
    {
        public override string Name => ":: Items ::: Reds :: H3AD-5T v2";
        public override ItemDef InternalPickup => RoR2Content.Items.FallBoots;

        public override string PickupText => "Increase jump height. Hold 'Interact' to slam down to the ground.";

        public override string DescText => "Increase <style=cIsUtility>jump height</style>. Creates a <style=cIsDamage>5m-100m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>1000%-" + d(maximumBaseDamage) + "</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>10</style> <style=cStack>(-50% per stack)</style> seconds.";

        [ConfigField("Maximum Base Damage", "Decimal.", 60f)]
        public static float maximumBaseDamage;

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
            On.EntityStates.Headstompers.HeadstompersFall.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersFall.maximumDamageCoefficient = maximumBaseDamage;
                orig(self);
            };
        }
    }
}