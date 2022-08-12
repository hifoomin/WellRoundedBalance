using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class VoidsentFlame : ItemBase
    {
        public static float Damage;
        public static float StackDamage;
        public static float Radius;
        public static float StackRadius;
        public static bool RemoveKnockback;

        public override string Name => ":: Items ::::::: Void Greens :: Voidsent Flame";
        public override string InternalPickupToken => "explodeOnDeathVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Upon hitting an enemy at or above <style=cIsDamage>100% health</style>, <style=cIsDamage>detonate</style> them in a <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> radius burst for <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(StackDamage) + " per stack)</style> base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        public override void Init()
        {
            Damage = ConfigOption(2.6f, "Base Damage", "Decimal. Vanilla is 2.6");
            StackDamage = ConfigOption(1.56f, "Stack Damage", "Decimal. Per Stack. Vanilla is 1.56");
            Radius = ConfigOption(12f, "Base Range", "Vanilla is 12");
            StackRadius = ConfigOption(2.4f, "Stack Range", "Per Stack. Vanilla is 2.4");
            RemoveKnockback = ConfigOption(false, "Remove Knockback?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += ChangeRadius;
            IL.RoR2.HealthComponent.TakeDamage += ChangeKnockback;
        }

        private void ChangeKnockback(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = RemoveKnockback ? 0f : 1000f;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Voidsent Flame Knockback hook");
            }
        }

        private void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(12f),
                    x => x.MatchLdcR4(2.4f)))
            {
                c.Next.Operand = Radius;
                c.Index += 1;
                c.Next.Operand = StackRadius;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Voidsent Flame Radius hook");
            }
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2.6f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1),
                    x => x.MatchSub(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Next.Operand = Damage;
                c.Index += 6;
                c.Next.Operand = StackDamage / Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Voidsent Flame Damage hook");
            }
        }
    }
}