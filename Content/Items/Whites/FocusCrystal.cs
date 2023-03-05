using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class FocusCrystal : ItemBase
    {
        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override string InternalPickupToken => "nearbyDamageBonus";

        public override string PickupText => "Deal bonus damage to nearby enemies.";

        public override string DescText =>
            StackDesc(damageIncrease, damageIncreaseStack, init => $"Increase damage to enemies within <style=cIsDamage>13m</style> by <style=cIsDamage>{d(init)}</style>{{Stack}}.", d);

        [ConfigField("Damage Increase", "Decimal.", 0.15f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.15f)]
        public static float damageIncreaseStack;

        [ConfigField("Damage Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIncreaseIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthCompoment_TakeDamage;
        }

        public static void HealthCompoment_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdloc(24)) && c.TryGotoNext(x => x.MatchStloc(6)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, 24);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(damageIncrease, damageIncreaseStack, stack, damageIncreaseIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Focus Crystal Damage hook");
        }
    }
}