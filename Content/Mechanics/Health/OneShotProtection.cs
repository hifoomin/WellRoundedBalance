using MonoMod.Cil;

namespace WellRoundedBalance.Mechanics.Health
{
    public class OneShotProtection : MechanicBase
    {
        public override string Name => ":: Mechanics :: One Shot Protection";

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
                Logger.LogError("Failed to apply One Shot Protection Time hook");
            }
        }
    }
}