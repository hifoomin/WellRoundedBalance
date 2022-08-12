using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class LysateCell : ItemBase
    {
        public static float CooldownReduction;

        public override string Name => ":: Items ::::::: Void Greens :: Lysate Cell";
        public override string InternalPickupToken => "equipmentMagazineVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Special skill</style>. <style=cIsVoid>Corrupts all Fuel Cells.</style>.";

        public override void Init()
        {
            CooldownReduction = ConfigOption(0.33f, "Cooldown Reduction", "Decimal. Vanilla is 0.33");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
        }

        private void ChangeCDR(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.67f)))
            {
                c.Next.Operand = 1f - CooldownReduction;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Lysate Cell Cooldown Reduction hook");
            }
        }
    }
}