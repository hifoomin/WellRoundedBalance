using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class EquipmentChargeCap : GlobalBase
    {
        public static int EquipChargeCap;
        public override string Name => ": Global :::::: Misc";

        public override void Init()
        {
            EquipChargeCap = ConfigOption(255, "Maximum Equipment Charges", "Vanilla is 255");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Inventory.GetEquipmentSlotMaxCharges += ChangeCap;
        }

        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(255)
            );
            c.Next.Operand = EquipChargeCap;
        }
    }
}