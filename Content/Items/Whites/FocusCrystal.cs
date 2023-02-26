using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class FocusCrystal : ItemBase
    {
        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override string InternalPickupToken => "nearbyDamageBonus";

        public override string PickupText => "Deal bonus damage to nearby enemies.";

        public override string DescText => "Increase damage to enemies within <style=cIsDamage>13m</style> by <style=cIsDamage>" + d(damageIncrease) + "</style> <style=cStack>(+" + d(damageIncrease) + " per stack)</style>.";

        [ConfigField("Damage Increase", "Decimal.", 0.15f)]
        public static float damageIncrease;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchStfld<DamageInfo>("damageColorIndex"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 5;
                c.Next.Operand = damageIncrease;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Focus Crystal Damage hook");
            }
        }
    }
}