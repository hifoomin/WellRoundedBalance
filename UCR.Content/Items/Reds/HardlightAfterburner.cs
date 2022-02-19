using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun
{
    public class HardlightAfterburner : ItemBase
    {
        public static int charges;
        public static float cdr;
        public override string Name => ":: Items ::: Reds :: Hardlight Afterburner";
        public override string InternalPickupToken => "utilitySkillMagazine";
        public override bool NewPickup => true;
        public override string PickupText => "Add " + charges + " extra charges of your Utility skill. Reduce Utility skill cooldown.";

        public override string DescText => "Add <style=cIsUtility>+" + charges + "</style> <style=cStack>(+" + charges + " per stack)</style> charges of your <style=cIsUtility>Utility skill</style>. <style=cIsUtility>Reduces Utility skill cooldown</style> by <style=cIsUtility>" + Mathf.Round(cdr * 100f) + "%</style>.";
        public override void Init()
        {
            charges = ConfigOption(2, "Charges", "Per Stack. Vanilla is 2");
            cdr = ConfigOption(0.3333334f, "Utility Cooldown Reduction", "Vanilla is ");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCharges;
        }
        public static void ChangeCharges(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.SkillLocator>("utility"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)
            );
            c.Index += 2;
            c.Next.Operand = charges;
        }
        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6666667f)
            );
            c.Next.Operand = 1f - cdr;
        }
    }
}
