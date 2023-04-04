using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class ClayDunestrider : EnemyBase<ClayDunestrider>
    {
        public override string Name => "::: Bosses :::: Clay Dunestrider";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.ClayBoss.Recover.FireTethers += Recover_FireTethers;
        }

        private void Recover_FireTethers(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<BullseyeSearch, EntityStates.ClayBoss.Recover, BullseyeSearch>>((search, self) =>
                {
                    if (self.GetTeam() == TeamIndex.Player)
                    {
                        search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                    }
                    return search;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Clay Dunestrider Suck hook");
            }
        }
    }
}