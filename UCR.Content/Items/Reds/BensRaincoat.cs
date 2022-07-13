using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class BensRaincoat : ItemBase
    {
        public static float BarrierGain;
        public static float RechargeTime;

        public override string Name => ":: Items ::: Reds :: Bens Raincoat";
        public override string InternalPickupToken => "immuneToDebuff";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Prevents <style=cIsUtility>1 <style=cStack>(+1 per stack)</style></style> <style=cIsDamage>debuff</style> and instead grants a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>" + d(BarrierGain) + "</style> of <style=cIsHealing>maximum health</style>. Recharges every <style=cIsUtility>" + RechargeTime + "</style> seconds.</style>.";

        public override void Init()
        {
            BarrierGain = ConfigOption(0.1f, "Barrier Gain", "Decimal. Vanilla is 0.1");
            ROSOption("Greens", 0f, 1f, 0.05f, "3");
            RechargeTime = ConfigOption(5f, "Recharge Time", "Vanilla is 5");
            ROSOption("Greens", 0f, 10f, 0.5f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)
            );
            c.Next.Operand = BarrierGain;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)
            );
            c.Next.Operand = RechargeTime;
        }
    }
}