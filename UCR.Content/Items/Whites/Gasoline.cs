using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class Gasoline : ItemBase
    {
        public static float ExplosionDamage;
        public static float BurnDamage;
        public static float Radius;
        public static float StackRadius;

        public override string Name => ":: Items : Whites :: Gasoline";
        public override string InternalPickupToken => "igniteOnKill";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> for <style=cIsDamage>" + d(ExplosionDamage) + "</style> base Damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(BurnDamage) + "</style> <style=cStack>(+" + d(BurnDamage / 2f) + " per stack)</style> base Damage.";

        public override void Init()
        {
            ExplosionDamage = ConfigOption(1.5f, "Damage Coefficient", "Decimal. Vanilla is 1.5");
            BurnDamage = ConfigOption(1.5f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 1.5");
            Radius = ConfigOption(12f, "Range", "Vanilla is 12");
            StackRadius = ConfigOption(4f, "Range", "Vanilla is 4");
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
            ILCursor c = new ILCursor(il);

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
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(1.5f)
            );
            c.Next.Operand = BurnDamage;
            c.Index += 1;
            c.Next.Operand = BurnDamage;
            // how the FUCK does this item work lmao
            // with this, it'd be 300% (+150% per stack) of burn instead of the 150% (+75% per stack) as listed on the wiki...
            // is burns Damage hardcoded and gasoline just extends the DoT duration for more ticks?
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);

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