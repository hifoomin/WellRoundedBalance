using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidWhites
{
    public class EncrustedKey : ItemBase
    {
        public static float Chance;

        public override string Name => ":: Items :::::: Void Whites :: Lost Seers Lenses";
        public override string InternalPickupToken => "critGlassesVoid";
        public override bool NewPickup => true;
        public override string PickupText => "Gain a " + Chance + "% chance to instantly kill a non-boss enemy. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
        public override string DescText => "Your attacks have a <style=cIsDamage>" + Chance + "%</style> <style=cStack>(+0.5% per stack)</style> chance to <style=cIsDamage>instantly kill</style> a <style=cIsDamage>non-Boss enemy</style>. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";

        public override void Init()
        {
            Chance = ConfigOption(0.5f, "Chance", "Per Stack. Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeChance;
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchMul(),
                    x => x.MatchLdarg(1)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Lost Seer's Lenses Chance hook");
            }
        }
    }
}