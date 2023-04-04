using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class FocusCrystal : ItemBase<FocusCrystal>
    {
        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override ItemDef InternalPickup => RoR2Content.Items.NearbyDamageBonus;

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
            int dmg = -1;
            c.TryGotoNext(x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.damage)), x => x.MatchStloc(out dmg));
            int idx = GetItemLoc(c, nameof(RoR2Content.Items.NearbyDamageBonus));
            if (idx != -1 && c.TryGotoNext(x => x.MatchStloc(dmg)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, dmg);
                c.Emit(OpCodes.Ldloc, idx);
                c.EmitDelegate<Func<float, int, float>>((orig, stack) => orig * (1f + StackAmount(damageIncrease, damageIncreaseStack, stack, damageIncreaseIsHyperbolic)));
            }
            else Logger.LogError("Failed to apply Focus Crystal Damage hook");
        }
    }
}