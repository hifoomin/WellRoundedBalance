﻿using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class SaferSpaces : ItemBase<SaferSpaces>
    {
        public override string Name => ":: Items :::::: Voids :: Safer Spaces";
        public override ItemDef InternalPickup => DLC1Content.Items.BearVoid;

        public override string PickupText => "Block the next source of damage. <style=cIsVoid>Corrupts all Tougher Times</style>.";
        public override string DescText => "<style=cIsHealing>Blocks</style> incoming damage once. Recharges after <style=cIsUtility>" + baseCooldown + " seconds</style> <style=cStack>(-" + Mathf.Round((1f - cooldownMultiplier) * 100f) + "% per stack)</style>. <style=cIsVoid>Corrupts all Tougher Times</style>.";

        [ConfigField("Base Cooldown", "Formula for cooldown: Base Cooldown * Cooldown Multiplier ^ Safer Spaces", 30f)]
        public static float baseCooldown;

        [ConfigField("Cooldown Multiplier", "Formula for cooldown: Base Cooldown * Cooldown Multiplier ^ Safer Spaces", 0.93f)]
        public static float cooldownMultiplier;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += ChangeCooldowns;
        }

        private void ChangeCooldowns(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(15f),
                    x => x.MatchLdcR4(0.9f)))
            {
                c.Next.Operand = baseCooldown;
                c.Index += 1;
                c.Next.Operand = cooldownMultiplier;
            }
            else
            {
                Logger.LogError("Failed to apply Safer Spaces Cooldown hook");
            }
        }
    }
}