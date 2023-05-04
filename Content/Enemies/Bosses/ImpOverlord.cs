using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class ImpOverlord : EnemyBase<ImpOverlord>
    {
        public override string Name => "::: Bosses :: Imp Overlord";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ImpBossMonster.BlinkState.OnEnter += BlinkState_OnEnter;
            On.EntityStates.ImpBossMonster.GroundPound.OnEnter += GroundPound_OnEnter;
            IL.EntityStates.ImpBossMonster.BlinkState.ExitCleanup += BlinkState_ExitCleanup;
        }

        private void BlinkState_ExitCleanup(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<BlastAttack>("falloffModel")))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 0;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Imp Overlord Blink Falloff hook");
            }
        }

        private void GroundPound_OnEnter(On.EntityStates.ImpBossMonster.GroundPound.orig_OnEnter orig, EntityStates.ImpBossMonster.GroundPound self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.ImpBossMonster.GroundPound.blastAttackRadius = 15f;
                EntityStates.ImpBossMonster.GroundPound.damageCoefficient = 1.4f;
            }
            orig(self);
        }

        private void BlinkState_OnEnter(On.EntityStates.ImpBossMonster.BlinkState.orig_OnEnter orig, EntityStates.ImpBossMonster.BlinkState self)
        {
            if (!Main.IsInfernoDef())
            {
                self.duration = 2.15f;
                self.exitDuration = 1.4f;
                self.destinationAlertDuration = 1.8f;
            }
            orig(self);
        }
    }
}