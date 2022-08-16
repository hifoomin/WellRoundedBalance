using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class BrilliantBehemoth : ItemBase
    {
        public static float Damage;
        public static float Radius;
        public static float StackRadius;
        public override string Name => ":: Items ::: Reds :: Brilliant Behemoth";
        public override string InternalPickupToken => "behemoth";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "All your <style=cIsDamage>attacks explode</style> in a <style=cIsDamage>" + Radius + "m </style> <style=cStack>(+" + StackRadius + "m per stack)</style> radius for a bonus <style=cIsDamage>" + d(Damage) + "</style> TOTAL damage to nearby enemies.";

        public override void Init()
        {
            Damage = ConfigOption(0.6f, "Damage Increase", "Decimal. Vanilla is 0.6");
            Radius = ConfigOption(4f, "Base Area of Effect", "Vanilla is 4");
            StackRadius = ConfigOption(2.5f, "Stack Area of Effect", "Per Stack. Vanilla is 2.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitAll += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Brilliant Behemoth Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1.5f),
                    x => x.MatchLdcR4(2.5f)))
            {
                c.Next.Operand = Radius - StackRadius;
                c.Index += 1;
                c.Next.Operand = StackRadius;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Brilliant Behemoth Radius hook");
            }
        }
    }
}