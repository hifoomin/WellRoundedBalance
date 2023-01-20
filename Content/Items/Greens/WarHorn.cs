using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Items.Greens
{
    public class WarHorn : ItemBase
    {
        public override string Name => ":: Items :: Greens :: War Horn";
        public override string InternalPickupToken => "energizedOnEquipmentUse";

        public override string PickupText => "Activating your Equipment gives you a burst of attack speed.";

        public override string DescText => "Activating your Equipment gives you <style=cIsDamage>+70% attack speed</style> for <style=cIsDamage>8s</style> <style=cStack>(+6s per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted;
        }

        private void EquipmentSlot_OnEquipmentExecuted(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(8),
                    x => x.MatchLdcI4(4)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 8);
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 6);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply War Horn Duration hook");
            }
        }
    }
}