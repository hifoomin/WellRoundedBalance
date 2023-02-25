using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class BisonSteak : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bison Steak";
        public override string InternalPickupToken => "flatHealth";

        public override string PickupText => "Gain 45 max health.";

        public override string DescText => "Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>" + maximumHealthGain + "</style> <style=cStack>(+" + maximumHealthGain + " per stack)</style>.";

        [ConfigField("Maximum Health Gain", "", 45f)]
        public static float maximumHealthGain;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(25f)))
            {
                c.Index += 1;
                c.Next.Operand = maximumHealthGain;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bison Steak Health hook");
            }
        }
    }
}