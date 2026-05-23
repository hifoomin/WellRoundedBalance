using System;
using RoR2.Items;

namespace WellRoundedBalance.Items.Whites
{
    public class EclipseLite : ItemBase<EclipseLite>
    {
        public override string Name => ":: Items : Whites :: Eclipse Lite";
        public override ItemDef InternalPickup => DLC3Content.Items.BarrierOnCooldown;

        public override string PickupText => "Gain a small temporary barrier when a skill comes off cooldown. ";

        public override string DescText => $"When a skill comes off cooldown, gain a <style=cIsDamage>temporary barrier</style> for <style=cIsHealing>{d(baseBarrier)}</style> <style=cStack>(+{d(barrierStack)} per stack)</style> of your maximum health per second of the skill's base cooldown.";

        [ConfigField("Base Barrier", 0.01f)]
        public static float baseBarrier;
        [ConfigField("Stack Barrier", 0.01f)]
        public static float barrierStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnSkillCooldown += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);
            c.TryGotoNext(x => x.MatchLdcR4(0.01f));
            c.Next.Operand = baseBarrier;
            c.Index++;
            c.TryGotoNext(x => x.MatchLdcR4(0.0025f));
            c.Next.Operand = barrierStack;
        }
    }
}