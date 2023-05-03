namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class Parent : EnemyBase<Parent>
    {
        public override string Name => ":: Minibosses :: Parent";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ParentMonster.GroundSlam.OnEnter += GroundSlam_OnEnter;
            Changes();
        }

        private void GroundSlam_OnEnter(On.EntityStates.ParentMonster.GroundSlam.orig_OnEnter orig, EntityStates.ParentMonster.GroundSlam self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.ParentMonster.GroundSlam.duration = 2.6f;
            orig(self);
        }

        private void Changes()
        {
            var parent = Utils.Paths.GameObject.ParentBody6.Load<GameObject>().GetComponent<CharacterBody>();
            parent.baseDamage = 14f;
            parent.levelDamage = 2.8f;
        }
    }
}