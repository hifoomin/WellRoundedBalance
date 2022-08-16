using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class FocusCrystal : ItemBase
    {
        public static float Damage;
        public static float Radius;

        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override string InternalPickupToken => "nearbyDamageBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increase damage to enemies within <style=cIsDamage>" + Radius + "m</style> by <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage", "Decimal. Per Stack. Vanilla is 0.2");
            Radius = ConfigOption(13f, "Range", "Vanilla is 13");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
            ChangeVisual();
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
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Focus Crystal Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchCallOrCallvirt<UnityEngine.Vector3>("get_sqrMagnitude"),
               x => x.MatchLdcR4(169f)))
            {
                c.Index += 1;
                c.Next.Operand = Radius * Radius;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Focus Crystal Radius hook");
            }
        }

        public static void ChangeVisual()
        {
            var focus = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
            float actualRange = Radius / 13f;
            focus.transform.localScale = new Vector3(actualRange, actualRange, actualRange);
        }
    }
}