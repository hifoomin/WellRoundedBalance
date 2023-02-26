namespace WellRoundedBalance.Items.Reds
{
    public class SpareDroneParts : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Spare Drone Parts";
        public override string InternalPickupToken => "droneWeapons";

        public override string PickupText => "Your drones fire faster, have less cooldowns, shoot missiles, and gain a bonus chaingun.";

        public override string DescText => "Gain <style=cIsDamage>Col. Droneman.</style> Drones gain <style=cIsDamage>+50%</style> <style=cStack>(+50% per stack)</style> attack speed and cooldown reduction. Drones gain <style=cIsDamage>10%</style> chance to fire a <style=cIsDamage>missile</style> on hit, dealing <style=cIsDamage>300%</style> TOTAL damage. Drones gain an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x30% damage</style>.";

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
            self.damageCoefficient = 0.3f;
            orig(self);
        }
    }
}