using BepInEx;
using R2API;
using R2API.Utils;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class CautiousSlug : Based
    {
        public static float regen;

        public override string Name => ":: Items : Whites :: Cautious Slug";
        public override string InternalPickupToken => "healWhileSafe";
        public override bool NewPickup => false;

        public override string PickupText => "";
        public override string DescText => "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+" + regen + " hp/s</style> <style=cStack>(+" + regen + " hp/s per stack)</style> while outside of combat.";
        public override void Init()
        {
            regen = ConfigOption(3f, "Regen", "Per Stack. Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealing;
        }
        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0),
                x => x.MatchBr(out _),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = regen;
        }
    }
}
