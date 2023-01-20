using MonoMod.Cil;

namespace WellRoundedBalance.Eclipse
{
    internal class Eclipse6 : GamemodeBase<Eclipse6>
    {
        // look at Mechanic>Bosses>Enrage
        public override string Name => ":: Gamemode : Eclipse";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += ChargeMegaNova_OnEnter;
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
                Main.WRBLogger.LogError("Failed to apply Eclipse 6 hook");
            }
        }

        private void ChargeMegaNova_OnEnter(On.EntityStates.VagrantMonster.ChargeMegaNova.orig_OnEnter orig, EntityStates.VagrantMonster.ChargeMegaNova self)
        {
            self.duration = Mathf.Max(3.2f, EntityStates.VagrantMonster.ChargeMegaNova.baseDuration / self.attackSpeedStat);
            // make nova enrage consistent kirn
            orig(self);
        }
    }
}