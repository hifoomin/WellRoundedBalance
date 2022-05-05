using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class LaserScope : ItemBase
    {
        public static float Damage;
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override string InternalPickupToken => "critDamage";
        public override bool NewPickup => true;
        public override string PickupText => "Your 'Critical Strikes' deal an additional " + d(Damage) + " damage.";

        public override string DescText => "<style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>" + d(Damage) + " damage</style><style=cStack>(+" + d(Damage) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(1f, "Crit Damage Increase", "Decimal. Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4()
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }
    }
}