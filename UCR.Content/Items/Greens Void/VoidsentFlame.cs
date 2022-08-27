using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class VoidsentFlame : ItemBase
    {
        public static float Damage;
        public static float StackDamage;
        public static float Radius;
        public static float StackRadius;
        public static float Force;
        public static int FalloffType;

        public override string Name => ":: Items :::::: Voids :: Voidsent Flame";
        public override string InternalPickupToken => "explodeOnDeathVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Upon hitting an enemy at <style=cIsDamage>100% health</style>, <style=cIsDamage>detonate</style> them in a <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> radius burst for <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(StackDamage) + " per stack)</style> base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        public override void Init()
        {
            Damage = ConfigOption(2.6f, "Base Damage", "Decimal. Vanilla is 2.6");
            StackDamage = ConfigOption(1.56f, "Stack Damage", "Decimal. Per Stack. Vanilla is 1.56");
            Radius = ConfigOption(12f, "Base Range", "Vanilla is 12");
            StackRadius = ConfigOption(2.4f, "Stack Range", "Per Stack. Vanilla is 2.4");
            Force = ConfigOption(1000f, "Force", "Vanilla is 1000");
            FalloffType = ConfigOption(2, "Falloff Type", "0 - None, 1 - Linear, 2 - Sweetspot.\nVanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = Force;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Voidsent Flame Knockback hook");
            }

            c.Index = 0;

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

            c.Index = 0;

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
            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2),
                x => x.MatchStfld("RoR2.DelayBlast", "falloffModel")))
            {
                c.Next.Operand = FalloffType;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Voidsent Flame Falloff hook");
            }
        }
    }
}