using MonoMod.Cil;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class MoreCommonLogs : EnemyBase<MoreCommonLogs>
    {
        public override string Name => "::: Bosses : More Common Logs";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
        }

        private void DeathRewards_OnKilledServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = 4.5f;
            }
            else
            {
                Logger.LogError("Failed to apply Boss Log Drop hook");
            }
        }
    }
}