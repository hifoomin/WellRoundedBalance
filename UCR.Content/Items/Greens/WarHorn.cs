using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Warhorn : Based
    {
        public static float aspd;
        public static int basedur;
        public static int stackdur;

        public override string Name => ":: Items :: Greens :: War Horn";
        public override string InternalPickupToken => "energizedOnEquipmentUse";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Activating your Equipment gives you <style=cIsDamage>+" + d(aspd) + " attack speed</style> for <style=cIsDamage>8s</style> <style=cStack>(+4s per stack)</style>.";


        public override void Init()
        {
            aspd = ConfigOption(0.7f, "Attack Speed", "Decimal. Vanilla is 0.7");
            /*
            basedur = ConfigOption(8, "Base Duration", "Vanilla is 8");
            stackdur = ConfigOption(4, "Stack Duration", "Per Stack. Vanilla is 4");
            */
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeAS;
            // IL.RoR2.EquipmentSlot.Execute += Warhorn.ChangeDuration;
        }
        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                //x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Energized"),
                //x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                //x => x.MatchBrfalse(out _),
                //x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.7f)
            );
            //c.Index += 4;
            c.Next.Operand = aspd;
        }
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(8),
                x => x.MatchLdcI4(4)
            );
            c.Next.Operand = basedur;
            c.Index += 1;
            c.Next.Operand = stackdur;
        }
    }
}
