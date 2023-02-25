using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class Medkit : ItemBase
    {
        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";

        public override string PickupText => "Receive a delayed heal after taking damage.";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>" + flatHealing + "</style> plus an additional <style=cIsHealing>" + d(percentHealing) + "<style=cStack> (+" + d(percentHealing) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        [ConfigField("Flat Healing", "", 20f)]
        public static float flatHealing;

        [ConfigField("Percent Healing", "Decimal.", 0.035f)]
        public static float percentHealing;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += CharacterBody_RemoveBuff_BuffIndex;
        }

        private void CharacterBody_RemoveBuff_BuffIndex(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(20f)))
            {
                c.Next.Operand = flatHealing;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Medkit Flat Healing hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Next.Operand = percentHealing;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Medkit Percent Healing hook");
            }
        }
    }
}