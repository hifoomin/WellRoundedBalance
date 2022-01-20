using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class EquipmentChargeCap
    {
        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(255)
            );
            c.Next.Operand = Main.EquipmentChargeCap.Value;
        }
    }
}
