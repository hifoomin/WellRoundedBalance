namespace WellRoundedBalance.Enemies.Bosses
{
    internal class AlloyWorshipUnit : EnemyBase
    {
        /* TODO:
            bring back spinning laser (lmaoo voidling's only good move isn't even original)
            add a component that does something with the solus probes possibly
        */
        public override string Name => "::: Bosses : Alloy Worship Unit";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.OnEnter += FireSuperDelayKnockup_OnEnter;
        }

        private void FireSuperDelayKnockup_OnEnter(On.EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.orig_OnEnter orig, EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup self)
        {
            EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.shieldDuration = 2.5f;
            orig(self);
        }
    }
}