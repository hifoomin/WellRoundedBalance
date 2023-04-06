using MonoMod.Cil;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse6 : GamemodeBase<Eclipse6>
    {
        // look at Mechanic>Bosses>Enrage
        public override string Name => ":: Gamemode : Eclipse 6";

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
                x => x.MatchLdcR4(0.8f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 6 hook");
            }
        }
    }
}