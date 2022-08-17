using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class LeptonLily : ItemBase
    {
        public static float PercentHeal;
        public static int Count;

        public override string Name => ":: Items :: Greens :: Lepton Lily";
        public override string InternalPickupToken => "tpHealingNova";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Release a <style=cIsHealing>healing nova </style>during the Teleporter event, <style=cIsHealing>healing</style> all nearby allies for <style=cIsHealing>50%</style> of their maximum health. Occurs <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style> times.";

        public override void Init()
        {
            PercentHeal = ConfigOption(0.5f, "Percent Healing", "Decimal. Vanilla is 0.5");
            Count = ConfigOption(1, "Makes 1 stack count as n", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            // IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.CalculatePulseCount += ChangeCount;
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.OnEnter += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = PercentHeal;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Lepton Lily Healing hook");
            }
        }

        private void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterMaster>("get_inventory"),
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "TPHealingNova"),
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, Inventory, int>>((useless, self) =>
                {
                    return self.GetItemCount(RoR2Content.Items.TPHealingNova) * Count;
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Lepton Lily Count hook");
            }
        }
    }
}