using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class SpareDroneParts : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Spare Drone Parts";
        public override string InternalPickupToken => "droneWeapons";

        public override string PickupText => "Your drones fire faster, have less cooldowns, shoot missiles, and gain a bonus chaingun.";

        public override string DescText => "Gain Col. Droneman. Drones gain +50% (+50% per stack) attack speed and cooldown reduction. Drones gain 10% chance to fire a missile on hit, dealing 300% TOTAL damage. Drones gain an automatic chain gun that deals 6x40% damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter += ChaingunChanges;
        }

        public static void ChaingunChanges(On.EntityStates.DroneWeaponsChainGun.FireChainGun.orig_OnEnter orig, EntityStates.DroneWeaponsChainGun.FireChainGun self)
        {
            self.additionalBounces = 0;
            self.damageCoefficient = 0.4f;
            orig(self);
        }
    }
}