using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class PredatoryInstincts : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Predatory Instincts";
        public override string InternalPickupToken => "attackSpeedOnCrit";

        public override string PickupText => "'Critical Strikes' increase attack speed up to 3 times.";

        public override string DescText => "Gain <style=cIsDamage>5% critical chance</style>. <style=cIsDamage>Critical strikes</style> increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(attackSpeedGainPerBuff) + "</style> up to <style=cIsDamage>3</style> <style=cStack>(+2 per stack)</style> times.";

        [ConfigField("Attack Speed Gain Per Buff", "", 0.16f)]
        public static float attackSpeedGainPerBuff;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdcR4(0.12f)))
            {
                c.Next.Operand = attackSpeedGainPerBuff;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Predatory Instincts Attack Speed hook");
            }
        }
    }
}