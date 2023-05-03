using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class Imp : EnemyBase<Imp>
    {
        public override string Name => ":: Enemies :: Imp";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ImpMonster.DoubleSlash.OnEnter += DoubleSlash_OnEnter;
        }

        private void DoubleSlash_OnEnter(On.EntityStates.ImpMonster.DoubleSlash.orig_OnEnter orig, EntityStates.ImpMonster.DoubleSlash self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.ImpMonster.DoubleSlash.walkSpeedPenaltyCoefficient = 0.66f;
            orig(self);
        }
    }
}