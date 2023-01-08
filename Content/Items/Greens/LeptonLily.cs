using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Greens
{
    internal class LeptonLily : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Lepton Lily";
        public override string InternalPickupToken => "tpHealingNova";

        public override string PickupText => "Periodically release a healing nova during the Teleporter event.";

        public override string DescText => "Release a <style=cIsHealing>healing nova </style>during the Teleporter event, <style=cIsHealing>healing</style> all nearby allies for <style=cIsHealing>30%</style> of their maximum health. Occurs <style=cIsHealing>2</style> <style=cStack>(+2 per stack)</style> times.";

        public override void Hooks()
        {
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.CalculatePulseCount += TeleporterHealNovaGeneratorMain_CalculatePulseCount;
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.OnEnter += TeleporterHealNovaPulse_OnEnter;
        }

        private void TeleporterHealNovaPulse_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = 0.3f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Lepton Lily Healing hook");
            }
        }

        private void TeleporterHealNovaGeneratorMain_CalculatePulseCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                x => x.MatchAdd(),
                x => x.MatchStloc(0)))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldc_I4, 2);
                c.Emit(OpCodes.Mul);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Lepton Lily Count hook");
            }
        }
    }
}