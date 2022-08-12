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
            RechargeTime = ConfigOption(5f, "Recharge Time", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = BarrierGain;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ben's Raincoat Barrier hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = RechargeTime;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ben's Raincoat Recharge hook");
            }
        }
    }
}