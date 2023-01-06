namespace WellRoundedBalance.Equipment
{
    public class TheCrowdfunder : EquipmentBase
    {
        public override string Name => ":: Equipment :: The Crowdfunder";
        public override string InternalPickupToken => "goldGat";

        public override string PickupText => "Toggle to fire. Costs gold per bullet.";

        public override string DescText => "Wind up a continuous barrage that shoots up to <style=cIsDamage>8 times per second</style>, dealing <style=cIsDamage>100% damage per shot</style> (extremely low). Costs $1 per bullet. Cost increases with level.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GoldGat.GoldGatFire.OnEnter += Changes;
        }

        private void Changes(On.EntityStates.GoldGat.GoldGatFire.orig_OnEnter orig, EntityStates.GoldGat.GoldGatFire self)
        {
            EntityStates.GoldGat.GoldGatFire.windUpDuration = 3f;
            EntityStates.GoldGat.GoldGatFire.minFireFrequency = 3f;
            EntityStates.GoldGat.GoldGatFire.maxFireFrequency = 8f;
            EntityStates.GoldGat.GoldGatFire.baseMoneyCostPerBullet = 1;
            EntityStates.GoldGat.GoldGatFire.procCoefficient = 1f;
            EntityStates.GoldGat.GoldGatFire.damageCoefficient = 1f;
            orig(self);
        }
    }
}