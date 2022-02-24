using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class HardlightAfterburner : ItemBase
    {
        public static int Charges;
        public static float CooldownReduction;
        public override string Name => ":: Items ::: Reds :: Hardlight Afterburner";
        public override string InternalPickupToken => "utilitySkillMagazine";
        public override bool NewPickup => true;
        public override string PickupText => "Add " + Charges + " extra Charges of your Utility skill. Reduce Utility skill Cooldown.";

        public override string DescText => "Add <style=cIsUtility>+" + Charges + "</style> <style=cStack>(+" + Charges + " per stack)</style> Charges of your <style=cIsUtility>Utility skill</style>. <style=cIsUtility>Reduces Utility skill Cooldown</style> by <style=cIsUtility>" + Mathf.Round(CooldownReduction * 100f) + "%</style>.";

        public override void Init()
        {
            Charges = ConfigOption(2, "Charges", "Per Stack. Vanilla is 2");
            CooldownReduction = ConfigOption(0.3333334f, "Utility Cooldown Reduction", "Vanilla is ");
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
            c.Next.Operand = Charges;
        }

        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6666667f)
            );
            c.Next.Operand = 1f - CooldownReduction;
        }
    }
}