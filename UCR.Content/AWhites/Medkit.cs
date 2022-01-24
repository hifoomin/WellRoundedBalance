using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Medkit
    {
        public static void ChangeFlatHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2.RoR2Content.Items), "Medkit"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>("GetItemCount"),
                x => x.MatchStloc(0),
                x => x.MatchLdcR4(20)
            ); ;
            c.Index += 2;
            c.Next.Operand = Main.MedkitFlatHealing.Value;
        }
        public static void ChangePercentHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt(typeof(RoR2.CharacterBody).GetMethod("get_maxHealth")),
                x => x.MatchLdcR4(0.05f)
            );
            c.Index += 2;
            c.Next.Operand = Main.MedkitPercentHealing.Value;
        }
    }
}
