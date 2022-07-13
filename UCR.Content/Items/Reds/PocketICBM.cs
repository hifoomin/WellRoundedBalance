using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class PocketICBM : ItemBase
    {
        public static int Missiles;
        public static float Damage;
        public override string Name => ":: Items ::: Reds :: Pocket ICBM";
        public override string InternalPickupToken => "moreMissile";
        public override bool NewPickup => true;
        public override string PickupText => "All Missile items deal more damage and fire an additional two missiles.";

        public override string DescText => "All missile items and equipment fire an additional <style=cIsDamage>2</style> <style=cIsDamage>missiles</style>. Increase missile damage by <style=cIsDamage>0%</style> <style=cStack>(+" + d(Damage) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(0.5f, "Missile Damage Increase", "Decimal. Per Stack. Vanilla is 0.5");
            ROSOption("Greens", 0f, 10f, 0.1f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.5f)
            );
            c.Index += 2;
            c.Next.Operand = Damage;
        }
    }
}