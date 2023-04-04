using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class BottledChaos : ItemBase<BottledChaos>
    {
        public override string Name => ":: Items ::: Reds :: Bottled Chaos";
        public override ItemDef InternalPickup => DLC1Content.Items.RandomEquipmentTrigger;

        public override string PickupText => "Activating your Equipment triggers " + randomEquipmentActivations + " additional, random Equipment effect" +
                                             (randomEquipmentActivations != 1 ? "s." : ".");

        public override string DescText => "Upon <style=cIsUtility>activating</style> your <style=cIsUtility>equipment</style>, trigger <style=cIsDamage>" + randomEquipmentActivations + "</style> <style=cStack>(+" + randomEquipmentActivations + " per stack)</style> <style=cIsDamage>random equipment</style> effect" +
                                            (randomEquipmentActivations != 1 ? "s." : ".");

        [ConfigField("Random Equipment Activations", 3)]
        public static int randomEquipmentActivations;

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
                c.Emit(OpCodes.Ldc_I4, randomEquipmentActivations);
                c.Emit(OpCodes.Mul);
            }
            else
            {
                Logger.LogError("Failed to apply Bottled Chaos Count hook");
            }
        }
    }
}