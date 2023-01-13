using MonoMod.Cil;

namespace WellRoundedBalance.Mechanic.Health
{
    public class OneShotProtection : MechanicBase
    {
        public override string Name => ":: Mechanic :: Health";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TriggerOneShotProtection += ChangeTime;
        }

        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = 0.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply One Shot Protection Time hook");
            }
        }
    }
}