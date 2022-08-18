using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidWhites
{
    public class SaferSpaces : ItemBase
    {
        public static float Cooldown;
        public static float StackCooldown;
        public static bool Formula;

        public override string Name => ":: Items :::::: Voids :: Safer Spaces";
        public override string InternalPickupToken => "bearVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsHealing>Blocks</style> incoming damage once. Recharges after <style=cIsUtility>" + Cooldown + " seconds</style> <style=cStack>(-" + d(StackCooldown) + " per stack)</style>. <style=cIsVoid>Corrupts all Tougher Times</style>.";

        public override void Init()
        {
            Cooldown = ConfigOption(15f, "Buff Cooldown", "Vanilla is 15");
            StackCooldown = ConfigOption(0.1f, "Buff Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.1");
            Formula = ConfigOption(true, "Buff Cooldown Formula", "Cooldown * (1 - StackCooldown) ^ Stack");
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
                c.Next.Operand = Cooldown;
                c.Index += 1;
                c.Next.Operand = 1f - StackCooldown;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Safer Spaces Cooldown hook");
            }
        }
    }
}