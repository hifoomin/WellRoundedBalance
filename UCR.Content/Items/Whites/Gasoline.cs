using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class Gasoline : ItemBase
    {
        public static float ExplosionDamage;
        public static int BurnDamage;
        public static float BurnDamageMultiplier;
        public static float Radius;
        public static float StackRadius;

        public override string Name => ":: Items : Whites :: Gasoline";
        public override string InternalPickupToken => "igniteOnKill";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> for <style=cIsDamage>" + d(ExplosionDamage) + "</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(BurnDamage * BurnDamageMultiplier) + "</style> <style=cStack>(+" + d(BurnDamage * BurnDamageMultiplier / 2f) + " per stack)</style> base damage.";

        public override void Init()
        {
            ExplosionDamage = ConfigOption(1.5f, "Explosion Damage", "Decimal. Vanilla is 1.5");
            BurnDamage = ConfigOption(2, "Burn Damage", "Per Stack. Vanilla is 2");
            BurnDamageMultiplier = ConfigOption(0.75f, "Burn Damage Multiplier", "Decimal. Vanilla is 0.75");
            Radius = ConfigOption(12f, "Base Range", "Vanilla is 12");
            StackRadius = ConfigOption(4f, "Stack Range", "Per Stack. Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeBurnDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeExplosionDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeRadius;
        }

        public static void ChangeExplosionDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchStloc(1),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 2;
            c.Next.Operand = ExplosionDamage;
        }

        public static void ChangeBurnDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchLdarg(1),
                x => x.MatchAdd(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.75f)
            );
            c.Next.Operand = BurnDamage;
            c.Index += 4;
            c.Next.Operand = BurnDamageMultiplier;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }
    }
}