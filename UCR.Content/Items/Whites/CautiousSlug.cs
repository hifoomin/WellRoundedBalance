using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class CautiousSlug : ItemBase
    {
        public static float Regen;

        public override string Name => ":: Items : Whites :: Cautious Slug";
        public override string InternalPickupToken => "healWhileSafe";
        public override bool NewPickup => false;

        public override string PickupText => "";
        public override string DescText => "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+" + Regen + " hp/s</style> <style=cStack>(+" + Regen + " hp/s per stack)</style> while outside of combat.";

        public override void Init()
        {
            Regen = ConfigOption(3f, "Regen", "Per Stack. Vanilla is 3");
            ROSOption("Whites", 0f, 6f, 0.1f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0),
                x => x.MatchBr(out _),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = Regen;
        }
    }
}