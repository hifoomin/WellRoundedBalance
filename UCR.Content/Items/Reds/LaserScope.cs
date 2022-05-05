using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class LaserScope : ItemBase
    {
        public static float Damage;
        public static float Radius;
        public static float StackRadius;
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override string InternalPickupToken => "critDamage";
        public override bool NewPickup => true;
        public override string PickupText => "Your 'Critical Strikes' deal an additional 100% damage.";

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
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeRadius;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6f)
            );
            c.Next.Operand = Damage;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(2.5f)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }
    }
}