using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class Bandolier : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Bandolier";
        public override string InternalPickupToken => "bandolier";

        public override string PickupText => "Chance on kill to drop an ammo pack that resets all skill cooldowns.";

        public override string DescText => "<style=cIsUtility>10%</style> <style=cStack>(+8% per stack)</style> chance on kill to drop an ammo pack that <style=cIsUtility>resets all skill cooldowns</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
        }

        public static void
        Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1)))
            {
                c.Index += 3;
                c.Next.Operand = 0.72f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bandolier Base hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchAdd(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.33f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.3f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bandolier Exponent hook");
            }
        }
    }
}