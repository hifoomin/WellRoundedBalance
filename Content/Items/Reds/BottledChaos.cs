using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class BottledChaos : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Bottled Chaos";
        public override string InternalPickupToken => "randomEquipmentTrigger";

        public override string PickupText => "Activating your Equipment triggers 3 additional, random Equipment effects.";

        public override string DescText => "Upon <style=cIsUtility>activating</style> your <style=cIsUtility>equipment</style>, trigger <style=cIsDamage>3</style> <style=cStack>(+3 per stack)</style> <style=cIsDamage>random equipment</style> effects.";

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
                x => x.MatchLdsfld("RoR2.DLC1Content/Items", "RandomEquipmentTrigger"),
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldc_I4, 3);
                c.Emit(OpCodes.Mul);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bottled Chaos Count hook");
            }
        }
    }
}