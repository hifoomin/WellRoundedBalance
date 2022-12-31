using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class SaferSpaces : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Safer Spaces";
        public override string InternalPickupToken => "bearVoid";

        public override string PickupText => "Block the next source of damage. <style=cIsVoid>Corrupts all Tougher Times</style>.";
        public override string DescText => "<style=cIsHealing>Blocks</style> incoming damage once. Recharges after <style=cIsUtility>30 seconds</style> <style=cStack>(-10% per stack)</style>. <style=cIsVoid>Corrupts all Tougher Times</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeCooldowns;
        }

        private void ChangeCooldowns(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(15f),
                    x => x.MatchLdcR4(0.9f)))
            {
                c.Next.Operand = 30f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Safer Spaces Cooldown hook");
            }
        }
    }
}