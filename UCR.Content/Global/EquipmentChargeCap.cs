using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class EquipmentChargeCap : GlobalBase
    {
        public static float ecc;
        public override string Name => ": Global :::: Maximum Equipment Charges";

        public override void Init()
        {
            ecc = ConfigOption(255, "Maximum Equipment Charges", "Vanilla is 255");
            base.Init();
        }
        public override void Hooks()
        {
            IL.RoR2.Inventory.GetEquipmentSlotMaxCharges += ChangeCap;
        }

        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(255)
            );
            c.Next.Operand = ecc;
        }
    }
}
