using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Whites
{
    public class Crowbar : ItemBase
    {
        public override string Name => ":: Items : Whites :: Crowbar";
        public override string InternalPickupToken => "crowbar";

        public override string PickupText => "Deal bonus damage to enemies above 85% health.";
        public override string DescText => "Deal <style=cIsDamage>+40%</style> <style=cStack>(+40% per stack)</style> damage to enemies above <style=cIsDamage>85% health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)))
            {
                c.Index += 1;
                c.Next.Operand = 0.4f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Crowbar Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdarg(0),
               x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
               x => x.MatchLdcR4(0.9f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.85f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Crowbar Threshold hook");
            }
        }
    }
}