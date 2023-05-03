using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    internal class LeptonLily : ItemBase<LeptonLily>
    {
        public override string Name => ":: Items :: Greens :: Lepton Lily";
        public override ItemDef InternalPickup => RoR2Content.Items.TPHealingNova;

        public override string PickupText => "Periodically release a healing nova during the Teleporter event.";

        public override string DescText => "Release a <style=cIsHealing>healing nova </style>during the Teleporter event, <style=cIsHealing>healing</style> all nearby allies for <style=cIsHealing>" + d(percentHealing) + "</style> of their maximum health. Occurs <style=cIsHealing>" + novaCountMultiplier + "</style> <style=cStack>(+" + novaCountMultiplier + " per stack)</style> times.";

        [ConfigField("Nova Count Multiplier", 2)]
        public static int novaCountMultiplier;

        [ConfigField("Percent Healing", "Decimal.", 0.35f)]
        public static float percentHealing;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.CalculatePulseCount += TeleporterHealNovaGeneratorMain_CalculatePulseCount;
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.OnEnter += TeleporterHealNovaPulse_OnEnter;
            HopooGames();
        }

        private void TeleporterHealNovaPulse_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = percentHealing;
            }
            else
            {
                Logger.LogError("Failed to apply Lepton Lily Healing hook");
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
                c.Emit(OpCodes.Ldc_I4, novaCountMultiplier);
                c.Emit(OpCodes.Mul);
            }
            else
            {
                Logger.LogError("Failed to apply Lepton Lily Count hook");
            }
        }

        private void HopooGames()
        {
            LanguageAPI.Add("ITEM_TPHEALINGNOVA_NAME", "Lepton Lily");
        }
    }
}