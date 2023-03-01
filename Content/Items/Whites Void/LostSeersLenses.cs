using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class EncrustedKey : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Lost Seers Lenses";
        public override string InternalPickupToken => "critGlassesVoid";

        public override string PickupText => "Gain a 0.45% chance to instantly kill a non-boss enemy. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
        public override string DescText => "Your attacks have a <style=cIsDamage>0.45%</style> <style=cStack>(+0.45% per stack)</style> chance to <style=cIsDamage>instantly kill</style> a <style=cIsDamage>non-Boss enemy</style>. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";

        [ConfigField("Instant Kill Chance", 0.45f)]
        public static float instantKillChance;

        public override void Init()
        {
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
                c.Next.Operand = instantKillChance;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Lost Seer's Lenses Chance hook");
            }
        }
    }
}